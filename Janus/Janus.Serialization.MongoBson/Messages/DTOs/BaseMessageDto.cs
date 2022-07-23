using MongoDB.Bson.Serialization.Attributes;

namespace Janus.Serialization.MongoBson.Messages.DTOs;

[BsonIgnoreExtraElements]
public class BaseMessageDto
{
    public string Preamble { get; set; }
    public string ExchangeId { get; set; }
    public string NodeId { get; set; }
}
