using Janus.Commons.SchemaModels;

namespace Janus.Serialization.MongoBson.SchemaModels.DTOs;

internal sealed class AttributeDto
{
    public string Name { get; set; }
    public DataTypes DataType { get; set; }
    public bool IsIdentity { get; set; }
    public bool IsNullable { get; set; }
    public int Ordinal { get; set; }
    public string Description { get; set; }
}
