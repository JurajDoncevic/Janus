using Janus.Commons.SchemaModels;

namespace Janus.Serialization.Avro.DataModels.DTOs;

public class TabularDataDto
{
    public Dictionary<string, DataTypes> AttributeDataTypes { get; set; } = new Dictionary<string, DataTypes>();
    public List<Dictionary<string, byte[]?>> AttributeValues { get; set; } = new List<Dictionary<string, byte[]?>>();
}
