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
    private readonly string _nodeId;
    /// <summary>
    /// Consctructor
    /// </summary>
    /// <param name="nodeId">Sender's node ID</param>
    /// <param name="dataSource">Data source to be sent</param>
    public SchemaResMessage(string nodeId, DataSource dataSource!!) : base(Preambles.SCHEMA_RESPONSE)
    {
        _dataSource = dataSource;
        _nodeId = nodeId;
    }
    [JsonConstructor]
    public SchemaResMessage(string exchangeId, string nodeId, DataSource dataSource!!) : base(exchangeId, Preambles.SCHEMA_RESPONSE)
    {
        _dataSource = dataSource;
        _nodeId= nodeId;
    }

    /// <summary>
    /// Data source given by sender
    /// </summary>
    public DataSource DataSource => _dataSource;

    /// <summary>
    /// Sender's node ID
    /// </summary>
    public string NodeId => _nodeId;
}


public static partial class MessageExtensions
{
#pragma warning disable
    public static DataResult<SchemaResMessage> ToSchemaResMessage(this byte[] bytes)
        => ResultExtensions.AsDataResult(
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