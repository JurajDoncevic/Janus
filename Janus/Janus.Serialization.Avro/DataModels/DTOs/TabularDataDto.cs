using Janus.Commons.SchemaModels;

namespace Janus.Serialization.Avro.DataModels.DTOs;

public sealed class TabularDataDto
{
    public string Name { get; set; }
    public Dictionary<string, DataTypes> AttributeDataTypes { get; set; } = new Dictionary<string, DataTypes>();
    public List<Dictionary<string, byte[]?>> AttributeValues { get; set; } = new List<Dictionary<string, byte[]?>>();
}
