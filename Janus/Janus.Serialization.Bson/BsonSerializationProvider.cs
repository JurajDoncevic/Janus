using FunctionalExtensions.Base.Results;
using Janus.Commons.CommandModels;
using Janus.Commons.Messages;
using Janus.Serialization.Bson.CommandModels;
using Janus.Serialization.Bson.DataModels;
using Janus.Serialization.Bson.Messages;
using Janus.Serialization.Bson.QueryModels;
using Janus.Serialization.Bson.SchemaModels;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Janus.Serialization.Bson;

/// <summary>
/// BSON format serialization provider
/// </summary>
public class BsonSerializationProvider : IBytesSerializationProvider
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
            var jsonNode = JsonSerializer.Deserialize<JsonNode>(messageBytes);
            return jsonNode["Preamble"].ToString();
        });

}
