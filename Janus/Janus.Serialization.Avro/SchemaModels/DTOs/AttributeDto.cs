using Janus.Commons.SchemaModels;

namespace Janus.Serialization.Avro.SchemaModels.DTOs;

internal class AttributeDto
{
    public string Name { get; set; }
    public DataTypes DataType { get; set; }
    public bool IsPrimaryKey { get; set; }
    public bool IsNullable { get; set; }
    public int Ordinal { get; set; }
}
