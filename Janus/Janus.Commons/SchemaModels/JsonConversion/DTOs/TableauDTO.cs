
namespace Janus.Commons.SchemaModels.JsonConversion.DTOs;

/// <summary>
/// DTO representation of <see cref="Tableau"/>
/// </summary>
internal class TableauDTO
{
    public string Name { get; set; }
    public List<AttributeDTO> Attributes { get; set; }
}
