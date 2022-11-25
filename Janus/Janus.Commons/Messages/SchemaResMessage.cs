using Janus.Commons.SchemaModels;

namespace Janus.Commons.Messages;

/// <summary>
/// Describes a SCHEMA_RES message
/// </summary>
public sealed class SchemaResMessage : BaseMessage
{
    private readonly DataSource _dataSource;
    /// <summary>
    /// Consctructor
    /// </summary>
    /// <param name="nodeId">Sender's node ID</param>
    /// <param name="dataSource">Data source to be sent</param>
    public SchemaResMessage(string nodeId, DataSource dataSource) : base(nodeId, Preambles.SCHEMA_RESPONSE)
    {
        _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
    }

    public SchemaResMessage(string exchangeId, string nodeId, DataSource dataSource) : base(exchangeId, nodeId, Preambles.SCHEMA_RESPONSE)
    {
        _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
    }

    /// <summary>
    /// Data source given by sender
    /// </summary>
    public DataSource DataSource => _dataSource;

    public override bool Equals(object? obj)
    {
        return obj is SchemaResMessage message &&
               _exchangeId == message._exchangeId &&
               _preamble == message._preamble &&
               NodeId == message.NodeId &&
               _dataSource.Equals(message._dataSource);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_exchangeId, _preamble, NodeId, _dataSource);
    }
}
