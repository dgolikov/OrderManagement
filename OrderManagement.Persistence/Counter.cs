using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OrderManagement.Persistence;

internal class Counter
{
    [BsonId]
    public string Id { get; set; } = null!;

    [BsonElement("seq")]
    public long Sequence { get; set; }
}
