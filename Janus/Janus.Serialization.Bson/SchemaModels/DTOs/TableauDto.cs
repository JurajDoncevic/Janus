
namespace Janus.Serialization.Bson.SchemaModels.DTOs;

/// <summary>
/// DTO representation of <see cref="Tableau"/>
/// </summary>
internal class TableauDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<AttributeDto> Attributes { get; set; }
    public HashSet<UpdateSetDto> UpdateSets { get; set; }
}
