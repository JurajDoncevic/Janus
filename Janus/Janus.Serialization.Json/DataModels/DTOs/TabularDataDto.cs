using Janus.Commons.SchemaModels;

namespace Janus.Serialization.Json.DataModels.DTOs;

public class TabularDataDto
{
    public string Name { get; set; }
    public Dictionary<string, DataTypes> AttributeDataTypes { get; set; } = new Dictionary<string, DataTypes>();
    public List<Dictionary<string, object?>> AttributeValues { get; set; } = new List<Dictionary<string, object?>>();
}
