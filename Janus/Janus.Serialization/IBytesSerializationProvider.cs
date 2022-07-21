using Janus.Commons.CommandModels;
using Janus.Commons.Messages;

namespace Janus.Serialization;

/// <summary>
/// Interface for serialization providers with byte format serializers
/// </summary>
public interface IBytesSerializationProvider
{
    public ITabularDataSerializer<byte[]> TabularDataSerializer { get; }
    public IQuerySerializer<byte[]> QuerySerializer { get; }
    public IDataSourceSerializer<byte[]> DataSourceSerializer { get; }
    public ICommandSerializer<DeleteCommand, byte[]> DeleteCommandSerializer { get; }
    public ICommandSerializer<InsertCommand, byte[]> InsertCommandSerializer { get; }
    public ICommandSerializer<UpdateCommand, byte[]> UpdateCommandSerializer { get; }
    public IMessageSerializer<HelloReqMessage, byte[]> HelloReqMessageSerializer { get; }
    public IMessageSerializer<HelloResMessage, byte[]> HelloResMessageSerializer { get; }
    public IMessageSerializer<ByeReqMessage, byte[]> ByeReqMessageSerializer { get; }
    public IMessageSerializer<CommandReqMessage, byte[]> CommandReqMessageSerializer { get; }
    public IMessageSerializer<CommandResMessage, byte[]> CommandResMessageSerializer { get; }
    public IMessageSerializer<QueryReqMessage, byte[]> QueryReqMessageSerializer { get; }
    public IMessageSerializer<QueryResMessage, byte[]> QueryResMessageSerializer { get; }
    public IMessageSerializer<SchemaReqMessage, byte[]> SchemaReqMessageSerializer { get; }
    public IMessageSerializer<SchemaResMessage, byte[]> SchemaResMessageSerializer { get; }

    /// <summary>
    /// Determines the message's preamble/type
    /// </summary>
    /// <param name="messageBytes"></param>
    /// <returns></returns>
    public Result<string> DetermineMessagePreamble(byte[] messageBytes);
}
