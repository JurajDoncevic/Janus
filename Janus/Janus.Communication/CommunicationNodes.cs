using TcpAdapters = Janus.Communication.NetworkAdapters.Tcp;
using Janus.Communication.Nodes;
using Janus.Communication.NetworkAdapters;
using Janus.Communication.Nodes.Implementations;

namespace Janus.Communication;

public static class CommunicationNodes
{
    /// <summary>
    /// Creates a Mask communication node over TCP
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static MaskCommunicationNode CreateTcpMaskCommunicationNode(CommunicationNodeOptions options!!)
        => new MaskCommunicationNode(options, new TcpAdapters.MaskNetworkAdapter(options.ListenPort));

    /// <summary>
    /// Creates a Mediator communication node over TCP
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static MediatorCommunicationNode CreateTcpMediatorCommunicationNode(CommunicationNodeOptions options!!)
        => new MediatorCommunicationNode(options, new TcpAdapters.MediatorNetworkAdapter(options.ListenPort));

    /// <summary>
    /// Creates a Wrapper communication node over TCP
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static WrapperCommunicationNode CreateTcpWrapperCommunicationNode(CommunicationNodeOptions options!!)
        => new WrapperCommunicationNode(options, new TcpAdapters.WrapperNetworkAdapter(options.ListenPort));


    /// <summary>
    /// Creates a Mask communication node over a network adapter
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static MaskCommunicationNode CreateMaskCommunicationNode(CommunicationNodeOptions options!!, IMaskNetworkAdapter networkAdapter)
        => new MaskCommunicationNode(options, networkAdapter);

    /// <summary>
    /// Creates a Mediator communication node over a network adapter
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static MediatorCommunicationNode CreateMediatorCommunicationNode(CommunicationNodeOptions options!!, IMediatorNetworkAdapter networkAdapter)
        => new MediatorCommunicationNode(options, networkAdapter);

    /// <summary>
    /// Creates a Wrapper communication node over a network adapter
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static WrapperCommunicationNode CreateWrapperCommunicationNode(CommunicationNodeOptions options!!, IWrapperNetworkAdapter networkAdapter)
        => new WrapperCommunicationNode(options, networkAdapter);
}
