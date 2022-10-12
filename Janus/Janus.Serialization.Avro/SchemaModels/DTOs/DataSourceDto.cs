
namespace Janus.Serialization.Avro.SchemaModels.DTOs;

/// <summary>
/// DTO representation of <see cref="DataSource"/>
/// </summary>
internal class DataSourceDto
{
    public string Name { get; set; }
    public List<SchemaDto> Schemas { get; set; }
    public string Version { get; set; }
    public string Description { get; set; }
}
