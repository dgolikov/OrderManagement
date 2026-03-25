using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using OrderManagement.Domain.Order;

namespace OrderManagement.Caching.Converters;

public class OrderConverter : JsonConverter<Order>
{
    private static readonly FieldInfo LineItemsField =
        typeof(Order).GetField("_lineItems", BindingFlags.NonPublic | BindingFlags.Instance)!;

    private static readonly PropertyInfo OrderNumberProperty =
        typeof(Order).GetProperty(nameof(Order.OrderNumber))!;

    public override Order Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        var userId = root.GetProperty("userId").GetGuid();
        var status = (OrderStatus)root.GetProperty("status").GetInt32();

        var result = Order.Create(userId, Enumerable.Empty<OrderLineItem>());

        Order order;
        if (result.IsSuccess)
        {
            order = result.Value;
        }
        else
        {
            var constructor = typeof(Order).GetTypeInfo()
                .DeclaredConstructors
                .First(c => c.GetParameters().Length == 2);

            order = (Order)constructor.Invoke(new object[] { userId, status });
        }

        if (root.TryGetProperty("lineItems", out var lineItemsProp))
        {
            var lineItems = JsonSerializer.Deserialize<List<OrderLineItem>>(lineItemsProp.GetRawText(), options);
            if (lineItems is not null)
            {
                LineItemsField.SetValue(order, lineItems);
            }
        }

        if (root.TryGetProperty("orderNumber", out var orderNumberProp))
        {
            OrderNumberProperty.SetValue(order, orderNumberProp.GetInt64());
        }

        if (root.TryGetProperty("status", out var statusProp))
        {
            order.SetStatus((OrderStatus)statusProp.GetInt32());
        }

        EntityJsonConverterBase.SetBaseProperties(order, root);

        return order;
    }

    public override void Write(Utf8JsonWriter writer, Order value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        EntityJsonConverterBase.WriteBaseProperties(writer, value);

        writer.WriteString("userId", value.UserId);
        writer.WriteNumber("status", (int)value.Status);
        writer.WriteNumber("orderNumber", value.OrderNumber);
        writer.WriteNumber("total", value.Total);

        writer.WritePropertyName("lineItems");
        JsonSerializer.Serialize(writer, value.LineItems, options);

        writer.WriteEndObject();
    }
}
