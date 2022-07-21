using Janus.Commons.CommandModels;

namespace Janus.Serialization;

/// <summary>
/// Command serializer interface
/// </summary>
/// <typeparam name="TCommand"></typeparam>
/// <typeparam name="TSerialized"></typeparam>
public interface ICommandSerializer<TCommand, TSerialized> where TCommand : BaseCommand
{
    /// <summary>
    /// Serializes a command
    /// </summary>
    /// <param name="command">Command to serialize</param>
    /// <returns>Serialized command</returns>
    public Result<TSerialized> Serialize(TCommand command);
    /// <summary>
    /// Deserializes a command
    /// </summary>
    /// <param name="serialized">Serialized command</param>
    /// <returns>Deserialized command</returns>
    public Result<TCommand> Deserialize(TSerialized serialized);
}
