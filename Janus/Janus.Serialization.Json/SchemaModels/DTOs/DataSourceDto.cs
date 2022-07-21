
namespace Janus.Serialization.Json.SchemaModels.DTOs;

/// <summary>
/// DTO representation of <see cref="DataSource"/>
/// </summary>
internal class DataSourceDto
{
    public string Name { get; set; }
    public List<SchemaDto> Schemas { get; set; }
}
