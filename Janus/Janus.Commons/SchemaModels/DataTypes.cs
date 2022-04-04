
using System.Text.Json.Serialization;

namespace Janus.Commons.SchemaModels;

/// <summary>
/// Supported data types
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DataTypes
{
    INT,
    DECIMAL,
    STRING,
    DATETIME,
    BOOLEAN,
    BINARY
}
