namespace Janus.Serialization.Avro.QueryModels.DTOs;

internal class ProjectionDto
{
    public HashSet<string> AttributeIds { get; set; } = new();
}
