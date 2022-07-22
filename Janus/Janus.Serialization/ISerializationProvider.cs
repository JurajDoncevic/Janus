using Janus.Commons.CommandModels;
using Janus.Commons.Messages;

namespace Janus.Serialization;
public interface ISerializationProvider<TSerialized>
{
    public ITabularDataSerializer<TSerialized> TabularDataSerializer { get; }
    public IQuerySerializer<TSerialized> QuerySerializer { get; }
    public IDataSourceSerializer<TSerialized> DataSourceSerializer { get; }
    public ICommandSerializer<DeleteCommand, TSerialized> DeleteCommandSerializer { get; }
    public ICommandSerializer<InsertCommand, TSerialized> InsertCommandSerializer { get; }
    public ICommandSerializer<UpdateCommand, TSerialized> UpdateCommandSerializer { get; }
    public IMessageSerializer<HelloReqMessage, TSerialized> HelloReqMessageSerializer { get; }
    public IMessageSerializer<HelloResMessage, TSerialized> HelloResMessageSerializer { get; }
    public IMessageSerializer<ByeReqMessage, TSerialized> ByeReqMessageSerializer { get; }
    public IMessageSerializer<CommandReqMessage, TSerialized> CommandReqMessageSerializer { get; }
    public IMessageSerializer<CommandResMessage, TSerialized> CommandResMessageSerializer { get; }
    public IMessageSerializer<QueryReqMessage, TSerialized> QueryReqMessageSerializer { get; }
    public IMessageSerializer<QueryResMessage, TSerialized> QueryResMessageSerializer { get; }
    public IMessageSerializer<SchemaReqMessage, TSerialized> SchemaReqMessageSerializer { get; }
    public IMessageSerializer<SchemaResMessage, TSerialized> SchemaResMessageSerializer { get; }

    /// <summary>
    /// Determines the message's preamble/type
    /// </summary>
    /// <param name="serializedMessage"></param>
    /// <returns></returns>
    public Result<string> DetermineMessagePreamble(TSerialized serializedMessage);
}
