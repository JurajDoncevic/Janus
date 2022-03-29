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

    /// <summary>
    /// ID of the message exchange (request and its response)
    /// </summary>
    public string ExchangeId => _exchangeId;
    /// <summary>
    /// Preamble AKA message type. Each message type has a specific name
    /// </summary>
    public string Preamble => _preamble;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="exchangeId">ID of the message exchange (request and its response)</param>
    /// <param name="preamble">Preamble AKA message type.</param>
    protected BaseMessage(string exchangeId, string preamble)
    {
        _exchangeId = exchangeId;
        _preamble = preamble;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="preamble">Preamble AKA message type.</param>
    protected BaseMessage(string preamble)
    {
        _exchangeId = Guid.NewGuid().ToString();
        _preamble = preamble;
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
    public static DataResult<string> DeterminePreambule(this byte[] bytes)
        => ResultExtensions.AsDataResult(
            () =>
            {
                var messageString = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                var preamble = Regex.Match(messageString, "(?<=\"Preamble\"\\s*:\\s*\").+?(?=\")").Value;
                return preamble ?? "UNKNOWN";
            });
}