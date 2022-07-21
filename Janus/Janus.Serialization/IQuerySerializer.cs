using Janus.Commons.QueryModels;

namespace Janus.Serialization;

/// <summary>
/// Query serializer interface
/// </summary>
/// <typeparam name="TSerialized"></typeparam>
public interface IQuerySerializer<TSerialized>
{
    /// <summary>
    /// Serializes a query
    /// </summary>
    /// <param name="query">Query to serialize</param>
    /// <returns>Serialized query</returns>
    public Result<TSerialized> Serialize(Query query);

    /// <summary>
    /// Deserializes a query
    /// </summary>
    /// <param name="serialized">Serialized query</param>
    /// <returns>Deserialized query</returns>
    public Result<Query> Deserialize(TSerialized serialized);
}
