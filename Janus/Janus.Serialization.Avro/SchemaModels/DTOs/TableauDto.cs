
namespace Janus.Serialization.Avro.SchemaModels.DTOs;

/// <summary>
/// DTO representation of <see cref="Tableau"/>
/// </summary>
internal class TableauDto
{
    public string Name { get; set; }
    public List<AttributeDto> Attributes { get; set; }
}
