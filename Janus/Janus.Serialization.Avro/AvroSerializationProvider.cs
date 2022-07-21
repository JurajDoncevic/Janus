using FunctionalExtensions.Base.Results;
using Janus.Commons.CommandModels;
using Janus.Commons.Messages;
using Janus.Serialization.Avro.CommandModels;
using Janus.Serialization.Avro.DataModels;
using Janus.Serialization.Avro.Messages;
using Janus.Serialization.Avro.Messages.DTOs;
using Janus.Serialization.Avro.QueryModels;
using Janus.Serialization.Avro.SchemaModels;
using SolTechnology.Avro;

namespace Janus.Serialization.Avro;

/// <summary>
/// Avro format serialization provider
/// </summary>
public class AvroSerializationProvider : IBytesSerializationProvider
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
            var schema = AvroConvert.GenerateSchema(typeof(BaseMessageDto));
            var messageJson = AvroConvert.Avro2Json(messageBytes, schema);
            string? preamble = System.Text.Json.JsonSerializer.Deserialize<BaseMessageDto>(messageJson)?.Preamble;
            return preamble ?? "UNKNOWN";
        });

    private bool IsKnownPreamble(string value)
        => value.Equals(Preambles.HELLO_REQUEST) ||
           value.Equals(Preambles.HELLO_RESPONSE) ||
           value.Equals(Preambles.BYE_REQUEST) ||
           value.Equals(Preambles.COMMAND_REQUEST) ||
           value.Equals(Preambles.COMMAND_RESPONSE) ||
           value.Equals(Preambles.QUERY_REQUEST) ||
           value.Equals(Preambles.QUERY_RESPONSE) ||
           value.Equals(Preambles.SCHEMA_REQUEST) ||
           value.Equals(Preambles.SCHEMA_RESPONSE);
}
