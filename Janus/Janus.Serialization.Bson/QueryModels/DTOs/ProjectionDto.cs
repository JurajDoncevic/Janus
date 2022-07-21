namespace Janus.Serialization.Bson.QueryModels.DTOs;

internal class ProjectionDto
{
    public HashSet<string> AttributeIds { get; set; } = new();
}
