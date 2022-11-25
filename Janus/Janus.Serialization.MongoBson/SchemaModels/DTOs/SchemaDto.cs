
namespace Janus.Serialization.MongoBson.SchemaModels.DTOs;

/// <summary>
/// /// <summary>
/// DTO representation of <see cref="SchemaModels"/>
/// </summary>
/// </summary>
internal sealed class SchemaDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<TableauDto> Tableaus { get; set; }
}
