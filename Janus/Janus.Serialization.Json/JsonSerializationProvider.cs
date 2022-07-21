using FunctionalExtensions.Base.Results;
using Janus.Commons.CommandModels;
using Janus.Commons.Messages;
using Janus.Serialization.Json.CommandModels;
using Janus.Serialization.Json.DataModels;
using Janus.Serialization.Json.Messages;
using Janus.Serialization.Json.QueryModels;
using Janus.Serialization.Json.SchemaModels;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Janus.Serialization.Json;

/// <summary>
/// JSON format serialization provider
/// </summary>
public class JsonSerializationProvider : IStringSerializationProvider
{
    public ITabularDataSerializer<string> TabularDataSerializer => new TabularDataSerializer();

    public IQuerySerializer<string> QuerySerializer => new QuerySerializer();

    public IDataSourceSerializer<string> DataSourceSerializer => new DataSourceSerializer();

    public ICommandSerializer<DeleteCommand, string> DeleteCommandSerializer => new DeleteCommandSerializer();

    public ICommandSerializer<InsertCommand, string> InsertCommandSerializer => new InsertCommandSerializer();

    public ICommandSerializer<UpdateCommand, string> UpdateCommandSerializer => new UpdateCommandSerializer();

    public IMessageSerializer<HelloReqMessage, string> HelloReqMessageSerializer => new HelloReqMessageSerializer();

    public IMessageSerializer<HelloResMessage, string> HelloResMessageSerializer => new HelloResMessageSerializer();

    public IMessageSerializer<ByeReqMessage, string> ByeReqMessageSerializer => new ByeReqMessageSerializer();

    public IMessageSerializer<CommandReqMessage, string> CommandReqMessageSerializer => new CommandReqMessageSerializer();

    public IMessageSerializer<CommandResMessage, string> CommandResMessageSerializer => new CommandResMessageSerializer();

    public IMessageSerializer<QueryReqMessage, string> QueryReqMessageSerializer => new QueryReqMessageSerializer();

    public IMessageSerializer<QueryResMessage, string> QueryResMessageSerializer => new QueryResMessageSerializer();

    public IMessageSerializer<SchemaReqMessage, string> SchemaReqMessageSerializer => new SchemaReqMessageSerializer();

    public IMessageSerializer<SchemaResMessage, string> SchemaResMessageSerializer => new SchemaResMessageSerializer();

    public Result<string> DetermineMessagePreamble(string messageBytes)
        => ResultExtensions.AsResult(() =>
        {
            var jsonNode = JsonSerializer.Deserialize<JsonNode>(messageBytes);
            return jsonNode["Preamble"].ToString();
        });
        
}
