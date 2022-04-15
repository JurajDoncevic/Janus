
namespace Janus.Commons.SchemaModels.JsonConversion.DTOs;

/// <summary>
/// DTO representation of <see cref="Attribute"/>
/// </summary>
internal class AttributeDTO
{
    public string Name { get; set; }
    public DataTypes DataType { get; set; }
    public bool IsPrimaryKey { get; set; }
    public bool IsNullable { get; set; }
    public int Ordinal { get; set; }
}
