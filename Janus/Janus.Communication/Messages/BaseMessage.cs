using System.Text.Json;
using System.Text.RegularExpressions;

namespace Janus.Communication.Messages;

/// <summary>
/// Abstract class for all system messages
/// </summary>
public abstract class BaseMessage
{
    protected readonly string _exchangeId;
    protected readonly string _preamble;
    private readonly string _nodeId;

    /// <summary>
    /// ID of the message exchange (request and its response)
    /// </summary>
    public string ExchangeId => _exchangeId;
    /// <summary>
    /// Preamble AKA message type. Each message type has a specific name
    /// </summary>
    public string Preamble => _preamble;

    /// <summary>
    /// Sender's node ID
    /// </summary>
    public string NodeId => _nodeId;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="exchangeId">ID of the message exchange (request and its response)</param>
    /// <param name="preamble">Preamble AKA message type.</param>
    /// <param name="nodeId">Sender's node ID</param>
    protected BaseMessage(string exchangeId, string nodeId, string preamble)
    {
        _exchangeId = exchangeId;
        _preamble = preamble;
        _nodeId = nodeId;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="preamble">Preamble AKA message type.</param>
    protected BaseMessage(string nodeId, string preamble)
    {
        _exchangeId = Guid.NewGuid().ToString();
        _preamble = preamble;
        _nodeId = nodeId;
    }

    /// <summary>
    /// Serialize message to binary JSON
    /// </summary>
    /// <returns></returns>
    public virtual byte[] ToBson()
        => Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(this, this.GetType()).ToCharArray()
            );
}

public static partial class MessageExtensions
{
    public static Result<string> DeterminePreambule(this byte[] bytes)
        => ResultExtensions.AsResult(
            () =>
            {
                var messageString = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                var preamble = Regex.Match(messageString, "(?<=\"Preamble\"\\s*:\\s*\").+?(?=\")").Value;
                return preamble ?? "UNKNOWN";
            });
}