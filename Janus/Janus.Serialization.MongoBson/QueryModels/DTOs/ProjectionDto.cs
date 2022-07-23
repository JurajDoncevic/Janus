namespace Janus.Serialization.MongoBson.QueryModels.DTOs;

internal class ProjectionDto
{
    public HashSet<string> AttributeIds { get; set; } = new();
}
