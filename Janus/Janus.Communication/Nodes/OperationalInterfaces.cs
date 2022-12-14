using Janus.Commons.CommandModels;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Communication.Nodes.Events;
using Janus.Communication.Remotes;

namespace Janus.Communication.Nodes;

public interface ISendsCommandReq
{
    /// <summary>
    /// Sends a COMMAND_REQ message to a remote point
    /// </summary>
    /// <param name="command">Command to send</param>
    /// <param name="remotePoint">Destination remote point</param>
    /// <returns>Result of commandoutcome</returns>
    public Task<Result> SendCommandRequest(BaseCommand command, RemotePoint remotePoint);
}

public interface ISendsCommandRes
{
    /// <summary>
    /// Sends a COMMAND_RES message to a remote point
    /// </summary>
    /// <param name="exchangeId">Exchange identifier</param>
    /// <param name="isSuccess">Indicator of command operation success</param>
    /// <param name="remotePoint">Destination remote point</param>
    /// <param name="outcomeDescription">Operation outcome description</param>
    /// <returns></returns>
    public Task<Result> SendCommandResponse(string exchangeId, bool isSuccess, RemotePoint remotePoint, string outcomeDescription = "");
}

public interface ISendsQueryReq
{
    /// <summary>
    /// Sends a QUERY_REQ message to a remote point
    /// </summary>
    /// <param name="query">Query to send</param>
    /// <param name="remotePoint">Destination remote point</param>
    /// <returns>Result of TabularData</returns>
    public Task<Result<TabularData>> SendQueryRequest(Query query, RemotePoint remotePoint);
}

public interface ISendsQueryRes
{
    /// <summary>
    /// Sends a QUERY_RES message to a remote point
    /// </summary>
    /// <param name="exchangeId">Exchange identifier</param>
    /// <param name="queryResult">Tabular data with query result. Null if failure</param>
    /// <param name="remotePoint">Destination remote point</param>
    /// <param name="outcomeDescription">Operation outcome description</param>
    /// <param name="blockNumber">Block number in response</param>
    /// <param name="totalBlocks">Total number of blocks to be expected</param>
    public Task<Result> SendQueryResponse(string exchangeId, TabularData? queryResult, RemotePoint remotePoint, string? outcomeDescription = null, int blockNumber = 1, int totalBlocks = 1);
}

public interface ISendsSchemaReq
{
    /// <summary>
    /// Sends a SCHEMA_REQ message to a remote point
    /// </summary>
    /// <param name="remotePoint">Destination remote point</param>
    /// <returns>Result of DataSource</returns>
    public Task<Result<DataSource>> SendSchemaRequest(RemotePoint remotePoint);
}

public interface ISendsSchemaRes
{

    /// <summary>
    /// Sends a SCHEMA_RES message to a remote point
    /// </summary>
    /// <param name="exchangeId">Exchange identifier</param>
    /// <param name="schema">Response schema. Null if failure</param>
    /// <param name="remotePoint">Destination remote point</param>
    /// <param name="outcomeDescription">Operation outcome description</param>
    /// <returns>Result of sending</returns>
    public Task<Result> SendSchemaResponse(string exchangeId, DataSource? schema, RemotePoint remotePoint, string? outcomeDescription = null);
}

public interface IReceivesCommandReq
{
    /// <summary>
    /// Invoked when a COMMAND_REQ message is received
    /// </summary>
    event EventHandler<CommandReqEventArgs>? CommandRequestReceived;
}
public interface IReceivesCommandRes
{
    /// <summary>
    /// Invoked when a COMMAND_RES message is received
    /// </summary>
    event EventHandler<CommandResEventArgs>? CommandResponseReceived;
}
public interface IReceivesQueryReq
{
    /// <summary>
    /// Invoked when a QUERY_REQ message is received
    /// </summary>
    event EventHandler<QueryReqEventArgs>? QueryRequestReceived;
}
public interface IReceivesQueryRes
{
    /// <summary>
    /// Invoked when a QUERY_RES message is received
    /// </summary>
    event EventHandler<QueryResEventArgs>? QueryResponseReceived;
}
public interface IReceivesSchemaReq
{
    /// <summary>
    /// Invoked when a SCHEMA_REQ message is received
    /// </summary>
    event EventHandler<SchemaReqEventArgs>? SchemaRequestReceived;
}
public interface IReceivesSchemaRes
{
    /// <summary>
    /// Invoked when a SCHEMA_RES message is received
    /// </summary>
    event EventHandler<SchemaResEventArgs>? SchemaResponseReceived;
}
