using System.Reflection;
using System.Text.Json;
using OrderManagement.Domain.Common.Abstractions;

namespace OrderManagement.Caching.Converters;

public abstract class EntityJsonConverterBase
{
    private static readonly PropertyInfo IdProperty =
        typeof(PersistableEntity).GetProperty(nameof(PersistableEntity.Id))!;

    private static readonly PropertyInfo CreatedOnProperty =
        typeof(PersistableEntity).GetProperty(nameof(PersistableEntity.CreatedOn))!;

    private static readonly PropertyInfo ModifiedOnProperty =
        typeof(PersistableEntity).GetProperty(nameof(PersistableEntity.ModifiedOn))!;

    public static void SetId(PersistableEntity entity, Guid id)
    {
        IdProperty.SetValue(entity, id);
    }

    public static void SetCreatedOn(PersistableEntity entity, DateTime? createdOn)
    {
        CreatedOnProperty.SetValue(entity, createdOn);
    }

    public static void SetModifiedOn(PersistableEntity entity, DateTime? modifiedOn)
    {
        ModifiedOnProperty.SetValue(entity, modifiedOn);
    }

    public static void SetBaseProperties(PersistableEntity entity, JsonElement root)
    {
        if (root.TryGetProperty("id", out var idProp))
        {
            SetId(entity, idProp.GetGuid());
        }

        if (root.TryGetProperty("createdOn", out var createdOnProp))
        {
            SetCreatedOn(entity, createdOnProp.GetDateTime());
        }

        if (root.TryGetProperty("modifiedOn", out var modifiedOnProp))
        {
            SetModifiedOn(entity, modifiedOnProp.GetDateTime());
        }
    }

    public static void WriteBaseProperties(Utf8JsonWriter writer, PersistableEntity entity)
    {
        writer.WriteString("id", entity.Id);
        if (entity.CreatedOn.HasValue)
            writer.WriteString("createdOn", entity.CreatedOn.Value);
        if (entity.ModifiedOn.HasValue)
            writer.WriteString("modifiedOn", entity.ModifiedOn.Value);
    }
}
