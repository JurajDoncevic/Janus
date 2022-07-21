using Janus.Commons.CommandModels;
using Janus.Commons.Messages;

namespace Janus.Serialization;

/// <summary>
/// Interface for serialization providers with string format serializers
/// </summary>
public interface IStringSerializationProvider
{
    public ITabularDataSerializer<string> TabularDataSerializer { get; }
    public IQuerySerializer<string> QuerySerializer { get; }
    public IDataSourceSerializer<string> DataSourceSerializer { get; }
    public ICommandSerializer<DeleteCommand, string> DeleteCommandSerializer { get; }
    public ICommandSerializer<InsertCommand, string> InsertCommandSerializer { get; }
    public ICommandSerializer<UpdateCommand, string> UpdateCommandSerializer { get; }
    public IMessageSerializer<HelloReqMessage, string> HelloReqMessageSerializer { get; }
    public IMessageSerializer<HelloResMessage, string> HelloResMessageSerializer { get; }
    public IMessageSerializer<ByeReqMessage, string> ByeReqMessageSerializer { get; }
    public IMessageSerializer<CommandReqMessage, string> CommandReqMessageSerializer { get; }
    public IMessageSerializer<CommandResMessage, string> CommandResMessageSerializer { get; }
    public IMessageSerializer<QueryReqMessage, string> QueryReqMessageSerializer { get; }
    public IMessageSerializer<QueryResMessage, string> QueryResMessageSerializer { get; }
    public IMessageSerializer<SchemaReqMessage, string> SchemaReqMessageSerializer { get; }
    public IMessageSerializer<SchemaResMessage, string> SchemaResMessageSerializer { get; }

    /// <summary>
    /// Determines the message's preamble/type
    /// </summary>
    /// <param name="messageBytes"></param>
    /// <returns></returns>
    public Result<string> DetermineMessagePreamble(string messageBytes);
}
