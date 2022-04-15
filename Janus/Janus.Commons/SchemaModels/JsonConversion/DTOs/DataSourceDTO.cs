
namespace Janus.Commons.SchemaModels.JsonConversion.DTOs;

/// <summary>
/// DTO representation of <see cref="DataSource"/>
/// </summary>
internal class DataSourceDTO
{
    public string Name { get; set; }
    public List<SchemaDTO> Schemas { get; set; }
}
