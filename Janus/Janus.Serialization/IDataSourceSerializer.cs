using Janus.Commons.SchemaModels;

namespace Janus.Serialization;

/// <summary>
/// Schema model serializer interface
/// </summary>
/// <typeparam name="TSerialized"></typeparam>
public interface IDataSourceSerializer<TSerialized>
{
    /// <summary>
    /// Serializes a data source
    /// </summary>
    /// <param name="dataSource">Data source to serialize</param>
    /// <returns>Serialized data source</returns>
    public Result<TSerialized> Serialize(DataSource dataSource);

    /// <summary>
    /// Deserializes a data source
    /// </summary>
    /// <param name="serialized">Serialized data source</param>
    /// <returns>Deserialized data source</returns>
    public Result<DataSource> Deserialize(TSerialized serialized);
}
