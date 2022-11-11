using Janus.Commons.SchemaModels;

namespace Janus.Serialization.MongoBson.DataModels.DTOs;

public class TabularDataDto
{
    public string Name { get; set; }
    public Dictionary<string, DataTypes> AttributeDataTypes { get; set; } = new Dictionary<string, DataTypes>();
    public List<Dictionary<string, byte[]?>> AttributeValues { get; set; } = new List<Dictionary<string, byte[]?>>();
}
