using Janus.Communication.Messages;
using Janus.Communication.Remotes;

namespace Janus.Communication.NetworkAdapters;

public interface INetworkAdapter : IDisposable
{
    /// <summary>
    /// Invoked when a HELLO_REQ message is received
    /// </summary>
    event EventHandler<HelloRequestReceivedEventArgs> HelloRequestMessageReceived;
    /// <summary>
    /// Invoked when a HELLO_RES message is received
    /// </summary>
    event EventHandler<HelloResponseReceivedEventArgs> HelloResponseMessageReceived;
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
}
