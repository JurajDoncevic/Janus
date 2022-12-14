using Janus.Commons.DataModels;
using Janus.Commons.SchemaModels;

namespace Janus.Commons.Messages;

/// <summary>
/// Describes a SCHEMA_RES message
/// </summary>
public sealed class SchemaResMessage : BaseMessage
{
    private readonly DataSource? _dataSource;
    private readonly string _outcomeDescription;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="nodeId">Sender's node ID</param>
    /// <param name="dataSource">Data source to be sent. Can be null on failure</param>
    /// <param name="outcomeDescription">Request operation outcome description</param>
    public SchemaResMessage(string nodeId, DataSource? dataSource, string? outcomeDescription = null) : base(nodeId, Preambles.SCHEMA_RESPONSE)
    {
        _dataSource = dataSource;
        _outcomeDescription = outcomeDescription ?? string.Empty;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="exchangeId">Exchange id</param>
    /// <param name="nodeId">Sender's node ID</param>
    /// <param name="dataSource">Data source to be sent. Can be null on failure</param>
    /// <param name="outcomeDescription">Request operation outcome description</param>
    public SchemaResMessage(string exchangeId, string nodeId, DataSource? dataSource, string? outcomeDescription = null) : base(exchangeId, nodeId, Preambles.SCHEMA_RESPONSE)
    {
        _dataSource = dataSource;
        _outcomeDescription = outcomeDescription ?? string.Empty;
    }

    /// <summary>
    /// Data source given by sender
    /// </summary>
    public Option<DataSource> DataSource => _dataSource is not null ? Option<DataSource>.Some(_dataSource) : Option<DataSource>.None;

    /// <summary>
    /// Indicates if the request succeded
    /// </summary>
    public bool IsSuccess => _dataSource is not null;

    /// <summary>
    /// Request operation outcome description
    /// </summary>
    public string OutcomeDescription => _outcomeDescription;

    public override bool Equals(object? obj)
    {
        return obj is SchemaResMessage message &&
               _exchangeId == message._exchangeId &&
               _preamble == message._preamble &&
                NodeId == message.NodeId &&
               (_dataSource?.Equals(message._dataSource) ?? message._dataSource == null) &&
               _outcomeDescription.Equals(message._outcomeDescription);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_exchangeId, _preamble, NodeId, _dataSource);
    }
}
