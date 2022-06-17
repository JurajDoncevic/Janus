using Janus.Commons.SchemaModels;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Janus.Communication.Messages;

/// <summary>
/// Describes a SCHEMA_RES message
/// </summary>
public class SchemaResMessage : BaseMessage
{
    private readonly DataSource _dataSource;
    /// <summary>
    /// Consctructor
    /// </summary>
    /// <param name="nodeId">Sender's node ID</param>
    /// <param name="dataSource">Data source to be sent</param>
    public SchemaResMessage(string nodeId, DataSource dataSource!!) : base(nodeId, Preambles.SCHEMA_RESPONSE)
    {
        _dataSource = dataSource;
    }
    [JsonConstructor]
    public SchemaResMessage(string exchangeId, string nodeId, DataSource dataSource!!) : base(exchangeId, nodeId, Preambles.SCHEMA_RESPONSE)
    {
        _dataSource = dataSource;
    }

    /// <summary>
    /// Data source given by sender
    /// </summary>
    public DataSource DataSource => _dataSource;
}


public static partial class MessageExtensions
{
#pragma warning disable
    public static Result<SchemaResMessage> ToSchemaResMessage(this byte[] bytes)
        => ResultExtensions.AsResult(
            () =>
            {
                var indexOfNullTerm = bytes.ToList().IndexOf(0x00);
                // sometimes the message is exactly as long as the byte array and there is no null term
                var bytesMessageLength = indexOfNullTerm > 0 ? indexOfNullTerm : bytes.Length;
                var messageString = Encoding.UTF8.GetString(bytes, 0, bytesMessageLength);
                var message = JsonSerializer.Deserialize<SchemaResMessage>(messageString);
                return message;
            });
#pragma warning enable


}