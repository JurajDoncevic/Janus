
using Janus.Commons.DataModels;
using System.Text.Json.Serialization;

namespace Janus.Commons.Messages;

/// <summary>
/// Describes a QUERY_RES message
/// </summary>
public sealed class QueryResMessage : BaseMessage
{
    private readonly TabularData? _tabularData;
    private readonly string _errorMessage;
    private readonly int _blockNumber;
    private readonly int _totalBlocks;
    private readonly bool _isFailure;

    /// <summary>
    /// Query result data
    /// </summary>
    public TabularData? TabularData => _tabularData;
    /// <summary>
    /// Error message (if it exists)
    /// </summary>
    public string ErrorMessage => _errorMessage;
    /// <summary>
    /// Response data block sequence number
    /// </summary>
    public int BlockNumber => _blockNumber;
    /// <summary>
    /// Response data block sequence size 
    /// </summary>
    public int TotalBlocks => _totalBlocks;
    /// <summary>
    /// Signals the query was a success
    /// </summary>
    public bool IsSuccess => !_isFailure;
    /// <summary>
    /// Signals the query failed
    /// </summary>
    public bool IsFailure => _isFailure;

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
        _isFailure = !string.IsNullOrEmpty(_errorMessage) || tabularData == null;
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
        _isFailure = !string.IsNullOrEmpty(_errorMessage) || tabularData == null;
    }

    public override bool Equals(object? obj)
    {
        return obj is QueryResMessage message &&
               _exchangeId == message._exchangeId &&
               _preamble == message._preamble &&
               NodeId == message.NodeId &&
               _errorMessage == message._errorMessage &&
               _blockNumber == message._blockNumber &&
               _totalBlocks == message._totalBlocks &&
               _isFailure == message._isFailure &&
               (_tabularData?.Equals(message._tabularData) ?? message._tabularData == null);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_exchangeId, _preamble, NodeId, _tabularData, _errorMessage, _blockNumber, _totalBlocks, _isFailure);
    }
}