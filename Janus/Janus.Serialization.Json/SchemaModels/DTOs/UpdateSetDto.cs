namespace Janus.Serialization.Json.SchemaModels.DTOs;
/// <summary>
/// DTO representation of an UpdateSet
/// </summary>
internal class UpdateSetDto
{
    public HashSet<string> AttributeIds { get; set; } = new HashSet<string>();
}
