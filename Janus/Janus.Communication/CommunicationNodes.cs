using Janus.Communication.NetworkAdapters;
using Janus.Communication.Nodes;
using Janus.Communication.Nodes.Implementations;
using Janus.Utils.Logging;
using TcpAdapters = Janus.Communication.NetworkAdapters.Tcp;

namespace Janus.Communication;

public static class CommunicationNodes
{
    /// <summary>
    /// Creates a Mask communication node over TCP
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static MaskCommunicationNode CreateTcpMaskCommunicationNode(
        CommunicationNodeOptions options!!,
        ILogger<MaskCommunicationNode>? nodeLogger = null,
        ILogger<TcpAdapters.MaskNetworkAdapter>? networkAdapterLogger = null)
        => new MaskCommunicationNode(
            options,
            new TcpAdapters.MaskNetworkAdapter(options.ListenPort, networkAdapterLogger));

    /// <summary>
    /// Creates a Mediator communication node over TCP
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static MediatorCommunicationNode CreateTcpMediatorCommunicationNode(
        CommunicationNodeOptions options!!,
        ILogger<MediatorCommunicationNode>? nodeLogger = null,
        ILogger<TcpAdapters.MediatorNetworkAdapter>? networkAdapterLogger = null)
        => new MediatorCommunicationNode(
            options,
            new TcpAdapters.MediatorNetworkAdapter(options.ListenPort, networkAdapterLogger));

    /// <summary>
    /// Creates a Wrapper communication node over TCP
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static WrapperCommunicationNode CreateTcpWrapperCommunicationNode(
        CommunicationNodeOptions options!!,
        ILogger<WrapperCommunicationNode>? nodeLogger = null,
        ILogger<TcpAdapters.WrapperNetworkAdapter>? networkAdapterLogger = null)
        => new WrapperCommunicationNode(
            options,
            new TcpAdapters.WrapperNetworkAdapter(options.ListenPort, networkAdapterLogger));


    /// <summary>
    /// Creates a Mask communication node over a network adapter
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static MaskCommunicationNode CreateMaskCommunicationNode(
        CommunicationNodeOptions options!!,
        IMaskNetworkAdapter networkAdapter,
        ILogger<MaskCommunicationNode>? nodeLogger = null)
        => new MaskCommunicationNode(options, networkAdapter);

    /// <summary>
    /// Creates a Mediator communication node over a network adapter
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static MediatorCommunicationNode CreateMediatorCommunicationNode(
        CommunicationNodeOptions options!!,
        IMediatorNetworkAdapter networkAdapter,
        ILogger<MediatorCommunicationNode>? nodeLogger = null)
        => new MediatorCommunicationNode(options, networkAdapter);

    /// <summary>
    /// Creates a Wrapper communication node over a network adapter
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static WrapperCommunicationNode CreateWrapperCommunicationNode(
        CommunicationNodeOptions options!!,
        IWrapperNetworkAdapter networkAdapter,
        ILogger<WrapperCommunicationNode>? nodeLogger = null)
        => new WrapperCommunicationNode(options, networkAdapter);
}
