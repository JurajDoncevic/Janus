namespace Janus.Serialization.Avro.SchemaModels.DTOs;
/// <summary>
/// DTO representation of an UpdateSet
/// </summary>
internal class UpdateSetDto
{
    public List<string> AttributeIds { get; set; } = new List<string>();
}
