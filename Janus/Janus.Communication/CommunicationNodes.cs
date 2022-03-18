using TcpAdapters = Janus.Communication.NetworkAdapters.Tcp;
using Janus.Communication.Nodes;

namespace Janus.Communication;

public static class CommunicationNodes
{
    /// <summary>
    /// Creates a Mask communication node over TCP
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static MaskCommunicationNode CreateTcpMaskCommunicationNode(CommunicationNodeOptions options!!)
        => new MaskCommunicationNode(options, new TcpAdapters.MaskNetworkAdapter(options.Port));

    /// <summary>
    /// Creates a Mediator communication node over TCP
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static MediatorCommunicationNode CreateTcpMediatorCommunicationNode(CommunicationNodeOptions options!!)
        => new MediatorCommunicationNode(options, new TcpAdapters.MediatorNetworkAdapter(options.Port));

    /// <summary>
    /// Creates a Wrapper communication node over TCP
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static WrapperCommunicationNode CreateTcpWrapperCommunicationNode(CommunicationNodeOptions options!!)
        => new WrapperCommunicationNode(options, new TcpAdapters.WrapperNetworkAdapter(options.Port));
}
