
using Janus.Commons.DataModels;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Janus.Communication.Messages;

/// <summary>
/// Describes a QUERY_RES message
/// </summary>
public class QueryResMessage : BaseMessage
{
    private readonly TabularData? _tabularData;
    private readonly string _errorMessage;
    private readonly int _blockNumber;
    private readonly int _totalBlocks;
    private readonly bool _isError;

    public TabularData? TabularData => _tabularData;

    public string ErrorMessage => _errorMessage;

    public int BlockNumber => _blockNumber;

    public int TotalBlocks => _totalBlocks;

    public bool IsError => _isError;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="exchangeId">ID of the message exchange (request and its response)</param>
    /// <param name="nodeId">Sender's node ID</param>
    [JsonConstructor]
    public QueryResMessage(string exchangeId, string nodeId, TabularData? tabularData, string? errorMessage = null, int blockNumber = 1, int totalBlocks = 1) : base(exchangeId, nodeId, Preambles.QUERY_RESPONSE)
    {
        _tabularData = tabularData;
        _errorMessage = errorMessage ?? string.Empty;
        _blockNumber = blockNumber;
        _totalBlocks = totalBlocks;
        _isError = !string.IsNullOrEmpty(_errorMessage) || tabularData == null;
    }
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="nodeId">Sender's node ID</param>
    public QueryResMessage(string nodeId, TabularData? tabularData, string errorMessage, int blockNumber = 1, int totalBlocks = 1) : base(nodeId, Preambles.QUERY_REQUEST)
    {
        _tabularData = tabularData;
        _errorMessage = errorMessage ?? string.Empty;
        _blockNumber = blockNumber;
        _totalBlocks = totalBlocks;
        _isError = !string.IsNullOrEmpty(_errorMessage) || tabularData == null;
    }
}

public static partial class MessageExtensions
{
#pragma warning disable
    public static DataResult<QueryResMessage> ToQueryResMessage(this byte[] bytes)
        => ResultExtensions.AsDataResult(
            () =>
            {
                var indexOfNullTerm = bytes.ToList().IndexOf(0x00);
                // sometimes the message is exactly as long as the byte array and there is no null term
                var bytesMessageLength = indexOfNullTerm > 0 ? indexOfNullTerm : bytes.Length;
                var messageString = Encoding.UTF8.GetString(bytes, 0, bytesMessageLength);
                var message = JsonSerializer.Deserialize<QueryResMessage>(messageString);
                return message;
            });
#pragma warning enable

}
