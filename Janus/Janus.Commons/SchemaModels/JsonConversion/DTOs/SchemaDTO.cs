
namespace Janus.Commons.SchemaModels.JsonConversion.DTOs;

/// <summary>
/// /// <summary>
/// DTO representation of <see cref="Schema"/>
/// </summary>
/// </summary>
internal class SchemaDTO
{
    public string Name { get; set; }
    public List<TableauDTO> Tableaus { get; set; }
}
