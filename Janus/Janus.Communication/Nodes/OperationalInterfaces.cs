using Janus.Commons.CommandModels;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Communication.Nodes.Events;
using Janus.Communication.Remotes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Communication.Nodes;

public interface ISendsCommandReq
{
    /// <summary>
    /// Sends a COMMAND_REQ message to a remote point
    /// </summary>
    /// <param name="message">COMMAND_REQ message</param>
    /// <param name="remotePoint">Destination remote point</param>
    /// <returns></returns>
    public Task<Result> SendCommandRequest(BaseCommand command, RemotePoint remotePoint);
}

public interface ISendsCommandRes
{
    /// <summary>
    /// Sends a COMMAND_RES message to a remote point
    /// </summary>
    /// <param name="message">COMMAND_RES message</param>
    /// <param name="remotePoint">Destination remote point</param>
    /// <returns></returns>
    public Task<Result> SendCommandResponse(bool isSuccess, RemotePoint remotePoint, string outcomeDescription = "");
}

public interface ISendsQueryReq
{
    /// <summary>
    /// Sends a QUERY_REQ message to a remote point
    /// </summary>
    /// <param name="message">QUERY_REQ message</param>
    /// <param name="remotePoint">Destination remote point</param>
    /// <returns></returns>
    public Task<Result> SendQueryRequest(Query query, RemotePoint remotePoint);
}

public interface ISendsQueryRes
{
    /// <summary>
    /// Sends a QUERY_RES message to a remote point
    /// </summary>
    /// <param name="message">QUERY_RES message</param>
    /// <param name="remotePoint">Destination remote point</param>
    /// <returns></returns>
    public Task<Result> SendQueryResponse(TabularData queryResult, RemotePoint remotePoint, string? errorMessage = null, int blockNumber = 1, int totalBlocks = 1);
}

public interface ISendsSchemaReq
{
    /// <summary>
    /// Sends a SCHEMA_REQ message to a remote point
    /// </summary>
    /// <param name="message">SCHEMA_REQ message</param>
    /// <param name="remotePoint">Destination remote point</param>
    /// <returns></returns>
    public Task<Result> SendSchemaRequest(RemotePoint remotePoint);
}

public interface ISendsSchemaRes
{
    /// <summary>
    /// Sends a SCHEMA_RES message to a remote point
    /// </summary>
    /// <param name="message">SCHEMA_RES message</param>
    /// <param name="remotePoint">Destination remote point</param>
    /// <returns></returns>
    public Task<Result> SendSchemaResponse(DataSource schema, RemotePoint remotePoint);
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
