using Janus.Communication.Nodes.Events;
using Janus.Communication.Remotes;

namespace Janus.Communication.Nodes;
public interface ICommunicationNode
{
    /// <summary>
    /// Invoked when a BYE_REQ message is received
    /// </summary>
    event EventHandler<ByeReqEventArgs>? ByeRequestReceived;

    /// <summary>
    /// Invoked when a HELLO_REQ message is received
    /// </summary>
    event EventHandler<HelloReqEventArgs>? HelloRequestReceived;

    /// <summary>
    /// Invoked when a HELLO_RES message is received
    /// </summary>
    event EventHandler<HelloResEventArgs>? HelloResponseReceived;

    /// <summary>
    /// Sends a hello to the remote point and waits for a response. Saves the remote point if hello is ok
    /// </summary>
    /// <param name="remotePoint"></param>
    /// <returns></returns>
    Task<Result<RemotePoint>> RegisterRemotePoint(UndeterminedRemotePoint remotePoint);

    /// <summary>
    /// Sends a bye to the remote point if it exists in the saved remote points. Removes the remote point from the saved remote points.
    /// </summary>
    /// <param name="remotePoint"></param>
    /// <returns></returns>
    Task<Result> SendBye(RemotePoint remotePoint);

    /// <summary>
    /// Sends a hello to the remote point and waits for a response. Doesn't save the remote point
    /// </summary>
    /// <param name="remotePoint"></param>
    /// <returns></returns>
    Task<Result<RemotePoint>> SendHello(RemotePoint remotePoint);
}