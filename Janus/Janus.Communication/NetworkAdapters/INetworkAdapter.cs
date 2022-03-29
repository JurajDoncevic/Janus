using Janus.Communication.Messages;
using Janus.Communication.NetworkAdapters.Events;
using Janus.Communication.Remotes;

namespace Janus.Communication.NetworkAdapters;

public interface INetworkAdapter : IDisposable
{
    /// <summary>
    /// Invoked when a HELLO_REQ message is received
    /// </summary>
    event EventHandler<HelloReqReceivedEventArgs> HelloRequestMessageReceived;
    /// <summary>
    /// Invoked when a HELLO_RES message is received
    /// </summary>
    event EventHandler<HelloResReceivedEventArgs> HelloResponseMessageReceived;
    /// <summary>
    /// Invoked when a BYE_REQ message is received
    /// </summary>
    event EventHandler<ByeReqReceivedEventArgs> ByeRequestMessageReceived;
    /// <summary>
    /// Sends a HELLO_REQ
    /// </summary>
    /// <param name="message">HELLO_REQ message</param>
    /// <param name="remotePoint">Destination remote point</param>
    /// <returns>Result of outcome</returns>
    Task<Result> SendHelloRequest(HelloReqMessage message, RemotePoint remotePoint);
    /// <summary>
    /// Send a HELLO_RES
    /// </summary>
    /// <param name="message">HELLO_RES message</param>
    /// <param name="remotePoint">Destination remote point</param>
    /// <returns>Result of outcome</returns>
    Task<Result> SendHelloResponse(HelloResMessage message, RemotePoint remotePoint);
    /// <summary>
    /// Send a BYE_REQ
    /// </summary>
    /// <param name="message">BYE_REQ message</param>
    /// <param name="remotePoint">Destination remote point</param>
    /// <returns>Result of outcome</returns>
    Task<Result> SendByeRequest(ByeReqMessage message, RemotePoint remotePoint);
}
