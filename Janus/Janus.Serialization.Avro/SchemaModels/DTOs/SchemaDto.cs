
namespace Janus.Serialization.Avro.SchemaModels.DTOs;

/// <summary>
/// /// <summary>
/// DTO representation of <see cref="SchemaModels"/>
/// </summary>
/// </summary>
internal class SchemaDto
{
    public string Name { get; set; }
    public List<TableauDto> Tableaus { get; set; }
    public string Description { get; set; }
}
