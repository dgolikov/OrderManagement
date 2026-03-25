using System.Text.Json;
using System.Text.Json.Serialization;
using OrderManagement.Domain.Product;

namespace OrderManagement.Caching.Converters;

public class ProductConverter : JsonConverter<Product>
{
    public override Product Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        var name = root.GetProperty("name").GetString()!;
        var price = root.GetProperty("price").GetDecimal();
        var sku = root.GetProperty("sku").GetString()!;
        var imageUrl = root.TryGetProperty("imageUrl", out var urlProp) ? urlProp.GetString() : null;

        var result = Product.Create(name, price, sku, imageUrl);
        if (!result.IsSuccess)
        {
            throw new JsonException($"Failed to deserialize Product: {result.Error?.Message ?? "Unknown error"}");
        }

        EntityJsonConverterBase.SetBaseProperties(result.Value, root);

        return result.Value;
    }

    public override void Write(Utf8JsonWriter writer, Product value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        EntityJsonConverterBase.WriteBaseProperties(writer, value);

        writer.WriteString("name", value.Name);
        writer.WriteNumber("price", value.Price);
        writer.WriteString("sku", value.SKU);
        if (value.ImageUrl is not null)
            writer.WriteString("imageUrl", value.ImageUrl);

        writer.WriteEndObject();
    }
}
