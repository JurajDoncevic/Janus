using Janus.Commons.DataModels;

namespace Janus.Serialization;

/// <summary>
/// Serializer interface for tabular data
/// </summary>
/// <typeparam name="TSerialized"></typeparam>
public interface ITabularDataSerializer<TSerialized>
{
    /// <summary>
    /// Serializes tabular data
    /// </summary>
    /// <param name="data">Tabular data to serialize</param>
    /// <returns>Serialized tabular data</returns>
    public Result<TSerialized> Serialize(TabularData data);

    /// <summary>
    /// Deserializes tabular data
    /// </summary>
    /// <param name="serialized">Serialized tabular data</param>
    /// <returns>Deserialized tabular data</returns>
    public Result<TabularData> Deserialize(TSerialized serialized);
}
