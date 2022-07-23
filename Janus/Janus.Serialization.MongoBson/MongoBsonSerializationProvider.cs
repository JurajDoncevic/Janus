using FunctionalExtensions.Base.Results;
using Janus.Commons.CommandModels;
using Janus.Commons.Messages;
using Janus.Serialization.MongoBson.CommandModels;
using Janus.Serialization.MongoBson.DataModels;
using Janus.Serialization.MongoBson.Messages;
using Janus.Serialization.MongoBson.Messages.DTOs;
using Janus.Serialization.MongoBson.QueryModels;
using Janus.Serialization.MongoBson.SchemaModels;
using MongoDB.Bson.Serialization;

namespace Janus.Serialization.MongoBson;

/// <summary>
/// MongoBson format serialization provider
/// </summary>
public class MongoBsonSerializationProvider : IBytesSerializationProvider
{
    public ITabularDataSerializer<byte[]> TabularDataSerializer => new TabularDataSerializer();

    public IQuerySerializer<byte[]> QuerySerializer => new QuerySerializer();

    public IDataSourceSerializer<byte[]> DataSourceSerializer => new DataSourceSerializer();

    public ICommandSerializer<DeleteCommand, byte[]> DeleteCommandSerializer => new DeleteCommandSerializer();

    public ICommandSerializer<InsertCommand, byte[]> InsertCommandSerializer => new InsertCommandSerializer();

    public ICommandSerializer<UpdateCommand, byte[]> UpdateCommandSerializer => new UpdateCommandSerializer();

    public IMessageSerializer<HelloReqMessage, byte[]> HelloReqMessageSerializer => new HelloReqMessageSerializer();

    public IMessageSerializer<HelloResMessage, byte[]> HelloResMessageSerializer => new HelloResMessageSerializer();

    public IMessageSerializer<ByeReqMessage, byte[]> ByeReqMessageSerializer => new ByeReqMessageSerializer();

    public IMessageSerializer<CommandReqMessage, byte[]> CommandReqMessageSerializer => new CommandReqMessageSerializer();

    public IMessageSerializer<CommandResMessage, byte[]> CommandResMessageSerializer => new CommandResMessageSerializer();

    public IMessageSerializer<QueryReqMessage, byte[]> QueryReqMessageSerializer => new QueryReqMessageSerializer();

    public IMessageSerializer<QueryResMessage, byte[]> QueryResMessageSerializer => new QueryResMessageSerializer();

    public IMessageSerializer<SchemaReqMessage, byte[]> SchemaReqMessageSerializer => new SchemaReqMessageSerializer();

    public IMessageSerializer<SchemaResMessage, byte[]> SchemaResMessageSerializer => new SchemaResMessageSerializer();

    public Result<string> DetermineMessagePreamble(byte[] messageBytes)
        => ResultExtensions.AsResult(() =>
        {
            var baseMessageDto = BsonSerializer.Deserialize<BaseMessageDto>(messageBytes);
            string preamble = baseMessageDto.Preamble;
            return preamble ?? "UNKNOWN";
        });

}
