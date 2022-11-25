
namespace Janus.Serialization.Bson.SchemaModels.DTOs;

/// <summary>
/// DTO representation of <see cref="DataSource"/>
/// </summary>
internal sealed class DataSourceDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Version { get; set; }
    public List<SchemaDto> Schemas { get; set; }
}
