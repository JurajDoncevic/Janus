namespace Janus.Serialization.Bson.QueryModels.DTOs;

internal sealed class ProjectionDto
{
    public HashSet<string> AttributeIds { get; set; } = new();
}
