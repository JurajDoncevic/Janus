
using Janus.Commons.CommandModels;
using Janus.Commons.CommandModels.JsonConversion;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Janus.Communication.Messages;

/// <summary>
/// Describes a COMMAND_REQ message
/// </summary>
[JsonConverter(typeof(CommandReqMessageJsonConverter))]
public class CommandReqMessage : BaseMessage
{
    private readonly BaseCommand _command;

    private readonly CommandMessageTypes _commandMessageType;

    public BaseCommand Command => _command;

    public CommandMessageTypes CommandMessageType => _commandMessageType;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="exchangeId">ID of the message exchange (request and its response)</param>
    /// <param name="nodeId">Sender's node ID</param>
    [JsonConstructor]
    public CommandReqMessage(string exchangeId, string nodeId, BaseCommand command!!) : base(exchangeId, nodeId, Preambles.COMMAND_REQUEST)
    {
        _command = command;
        _commandMessageType = command.DetermineMessageType();
    }
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="nodeId">Sender's node ID</param>
    public CommandReqMessage(string nodeId, BaseCommand command) : base(nodeId, Preambles.COMMAND_REQUEST)
    {
        _command = command;
        _commandMessageType = command.DetermineMessageType();
    }
}

public static partial class MessageExtensions
{
#pragma warning disable
    public static DataResult<CommandReqMessage> ToCommandReqMessage(this byte[] bytes)
        => ResultExtensions.AsDataResult(
            () =>
            {
                var indexOfNullTerm = bytes.ToList().IndexOf(0x00);
                // sometimes the message is exactly as long as the byte array and there is no null term
                var bytesMessageLength = indexOfNullTerm > 0 ? indexOfNullTerm : bytes.Length;
                var messageString = Encoding.UTF8.GetString(bytes, 0, bytesMessageLength);
                var message = JsonSerializer.Deserialize<CommandReqMessage>(messageString);
                return message;
            });
#pragma warning enable


}

internal class CommandReqMessageJsonConverter : JsonConverter<CommandReqMessage>
{
    public override CommandReqMessage? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jsonDocument = JsonDocument.ParseValue(ref reader);

        var exchangeId = jsonDocument.RootElement.GetProperty("ExchangeId").GetString();
        var nodeId = jsonDocument.RootElement.GetProperty("NodeId").GetString();
        var preamble = jsonDocument.RootElement.GetProperty("Preamble").GetString();
        if (!preamble.Equals(Preambles.COMMAND_REQUEST))
            throw new Exception($"Error during COMMAND_REQ deserialization. Received wrong preamble: {preamble}");

        var commandMessageType = jsonDocument.RootElement.GetProperty("CommandMessageType").Deserialize<CommandMessageTypes>();
        var command =
            (BaseCommand)jsonDocument.RootElement.GetProperty("Command").Deserialize(commandMessageType.DetermineCommandType());

        return new CommandReqMessage(exchangeId, nodeId, command);
    }

    public override void Write(Utf8JsonWriter writer, CommandReqMessage value, JsonSerializerOptions options)
    {
        var dto = new
        {
            value.Preamble,
            value.ExchangeId,
            value.NodeId,
            value.CommandMessageType,
            value.Command
        };
        var jsonNode = JsonSerializer.SerializeToNode(dto);
        var str = jsonNode.ToJsonString();
        
        jsonNode["Command"] = JsonSerializer.SerializeToNode(value.Command, value.Command.GetType(), options);

        writer.WriteRawValue(jsonNode.ToJsonString());
    }
}
