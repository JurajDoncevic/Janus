using Janus.Communication.Messages;
using Janus.Communication.NetworkAdapters.Events;
using Janus.Communication.Remotes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Communication.NetworkAdapters;

public interface ISendsCommandReq
{
    /// <summary>
    /// Sends a COMMAND_REQ message to a remote point
    /// </summary>
    /// <param name="message">COMMAND_REQ message</param>
    /// <param name="remotePoint">Destination remote point</param>
    /// <returns></returns>
    public Result SendCommandRequest(CommandReqMessage message, RemotePoint remotePoint);
}

public interface ISendsCommandRes
{
    /// <summary>
    /// Sends a COMMAND_RES message to a remote point
    /// </summary>
    /// <param name="message">COMMAND_RES message</param>
    /// <param name="remotePoint">Destination remote point</param>
    /// <returns></returns>
    public Result SendCommandResponse(CommandResMessage message, RemotePoint remotePoint);
}

public interface ISendsQueryReq
{
    /// <summary>
    /// Sends a QUERY_REQ message to a remote point
    /// </summary>
    /// <param name="message">QUERY_REQ message</param>
    /// <param name="remotePoint">Destination remote point</param>
    /// <returns></returns>
    public Result SendQueryRequest(QueryReqMessage message, RemotePoint remotePoint);
}

public interface ISendsQueryRes
{
    /// <summary>
    /// Sends a QUERY_RES message to a remote point
    /// </summary>
    /// <param name="message">QUERY_RES message</param>
    /// <param name="remotePoint">Destination remote point</param>
    /// <returns></returns>
    public Result SendQueryResponse(QueryResMessage message, RemotePoint remotePoint);
}

public interface ISendsSchemaReq
{
    /// <summary>
    /// Sends a SCHEMA_REQ message to a remote point
    /// </summary>
    /// <param name="message">SCHEMA_REQ message</param>
    /// <param name="remotePoint">Destination remote point</param>
    /// <returns></returns>
    public Result SendSchemaRequest(SchemaReqMessage message, RemotePoint remotePoint);
}

public interface ISendsSchemaRes
{
    /// <summary>
    /// Sends a SCHEMA_RES message to a remote point
    /// </summary>
    /// <param name="message">SCHEMA_RES message</param>
    /// <param name="remotePoint">Destination remote point</param>
    /// <returns></returns>
    public Result SendSchemaResponse(SchemaResMessage message, RemotePoint remotePoint);
}

public interface IReceivesCommandReq
{
    /// <summary>
    /// Invoked when a COMMAND_REQ message is received
    /// </summary>
    event EventHandler<CommandReqReceivedEventArgs>? CommandRequestReceived;
}
public interface IReceivesCommandRes
{
    /// <summary>
    /// Invoked when a COMMAND_RES message is received
    /// </summary>
    event EventHandler<CommandResReceivedEventArgs>? CommandResponseReceived;
}
public interface IReceivesQueryReq
{
    /// <summary>
    /// Invoked when a QUERY_REQ message is received
    /// </summary>
    event EventHandler<QueryReqReceivedEventArgs>? QueryRequestReceived;
}
public interface IReceivesQueryRes
{
    /// <summary>
    /// Invoked when a QUERY_RES message is received
    /// </summary>
    event EventHandler<QueryResReceivedEventArgs>? QueryResponseReceived;
}
public interface IReceivesSchemaReq
{
    /// <summary>
    /// Invoked when a SCHEMA_REQ message is received
    /// </summary>
    event EventHandler<SchemaReqReceivedEventArgs>? SchemaRequestReceived;
}
public interface IReceivesSchemaRes
{
    /// <summary>
    /// Invoked when a SCHEMA_RES message is received
    /// </summary>
    event EventHandler<SchemaResReceivedEventArgs>? SchemaResponseReceived;
}