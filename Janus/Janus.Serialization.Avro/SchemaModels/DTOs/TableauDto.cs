
namespace Janus.Serialization.Avro.SchemaModels.DTOs;

/// <summary>
/// DTO representation of <see cref="Tableau"/>
/// </summary>
internal sealed class TableauDto
{
    public string Name { get; set; }
    public List<AttributeDto> Attributes { get; set; }
    public string Description { get; set; }

    public List<UpdateSetDto> UpdateSets { get; set; }
}
