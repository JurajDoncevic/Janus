using Janus.Communication.NetworkAdapters;
using Janus.Communication.Nodes;
using Janus.Communication.Nodes.Implementations;
using Janus.Serialization;
using Janus.Logging;
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
        IBytesSerializationProvider serializationProvider!!,
        ILogger? logger = null)
        => new MaskCommunicationNode(
            options,
            new TcpAdapters.MaskNetworkAdapter(options.ListenPort, serializationProvider, logger),
            logger);

    /// <summary>
    /// Creates a Mediator communication node over TCP
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static MediatorCommunicationNode CreateTcpMediatorCommunicationNode(
        CommunicationNodeOptions options!!,
        IBytesSerializationProvider serializationProvider!!,
        ILogger? logger = null)
        => new MediatorCommunicationNode(
            options,
            new TcpAdapters.MediatorNetworkAdapter(options.ListenPort, serializationProvider, logger),
            logger);

    /// <summary>
    /// Creates a Wrapper communication node over TCP
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static WrapperCommunicationNode CreateTcpWrapperCommunicationNode(
        CommunicationNodeOptions options!!,
        IBytesSerializationProvider serializationProvider!!,
        ILogger? logger = null)
        => new WrapperCommunicationNode(
            options,
            new TcpAdapters.WrapperNetworkAdapter(options.ListenPort, serializationProvider, logger),
            logger);


    /// <summary>
    /// Creates a Mask communication node over a network adapter
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static MaskCommunicationNode CreateMaskCommunicationNode(
        CommunicationNodeOptions options!!,
        IMaskNetworkAdapter networkAdapter,
        ILogger? logger = null)
        => new MaskCommunicationNode(options, networkAdapter, logger);

    /// <summary>
    /// Creates a Mediator communication node over a network adapter
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static MediatorCommunicationNode CreateMediatorCommunicationNode(
        CommunicationNodeOptions options!!,
        IMediatorNetworkAdapter networkAdapter,
        ILogger? logger = null)
        => new MediatorCommunicationNode(options, networkAdapter, logger);

    /// <summary>
    /// Creates a Wrapper communication node over a network adapter
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static WrapperCommunicationNode CreateWrapperCommunicationNode(
        CommunicationNodeOptions options!!,
        IWrapperNetworkAdapter networkAdapter,
        ILogger? logger = null)
        => new WrapperCommunicationNode(options, networkAdapter, logger);
}
