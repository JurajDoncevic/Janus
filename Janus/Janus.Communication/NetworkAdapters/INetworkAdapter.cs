using Janus.Commons.Messages;
using Janus.Communication.NetworkAdapters.Events;
using Janus.Communication.Remotes;

namespace Janus.Communication.NetworkAdapters;

public interface INetworkAdapter : IDisposable
{
    /// <summary>
    /// Invoked when a HELLO_REQ message is received
    /// </summary>
    event EventHandler<HelloReqReceivedEventArgs>? HelloRequestReceived;
    /// <summary>
    /// Invoked when a HELLO_RES message is received
    /// </summary>
    event EventHandler<HelloResReceivedEventArgs>? HelloResponseReceived;
    /// <summary>
    /// Invoked when a BYE_REQ message is received
    /// </summary>
    event EventHandler<ByeReqReceivedEventArgs>? ByeRequestReceived;
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

    /// <summary>
    /// Starts the network adapter by hand
    /// </summary>
    /// <returns><c>true</c> on success, else <c>false</c></returns>
    public bool StartAdapter();

    /// <summary>
    /// Stops the network adapter
    /// </summary>
    /// <returns><c>true</c> on success, else <c>false</c></returns>
    public bool StopAdapter();
}
