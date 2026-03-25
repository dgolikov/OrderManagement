using System.Text.Json;
using System.Text.Json.Serialization;
using OrderManagement.Domain.User;

namespace OrderManagement.Caching.Converters;

public class UserConverter : JsonConverter<User>
{
    public override User Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        var firstName = root.TryGetProperty("firstName", out var fn) ? fn.GetString() ?? string.Empty : string.Empty;
        var lastName = root.TryGetProperty("lastName", out var ln) ? ln.GetString() ?? string.Empty : string.Empty;
        var email = root.GetProperty("email").GetString()!;
        var passwordHash = root.GetProperty("passwordHash").GetString()!;
        var salt = root.GetProperty("salt").GetString()!;

        var result = Domain.User.User.Create(firstName, lastName, email, passwordHash, salt);
        if (!result.IsSuccess)
        {
            throw new JsonException($"Failed to deserialize User: {result.Error?.Message ?? "Unknown error"}");
        }

        EntityJsonConverterBase.SetBaseProperties(result.Value, root);

        return result.Value;
    }

    public override void Write(Utf8JsonWriter writer, User value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        EntityJsonConverterBase.WriteBaseProperties(writer, value);

        writer.WriteString("firstName", value.FirstName);
        writer.WriteString("lastName", value.LastName);
        writer.WriteString("email", value.Email);
        writer.WriteString("passwordHash", value.PasswordHash);
        writer.WriteString("salt", value.Salt);

        writer.WriteEndObject();
    }
}
