using Janus.Commons.Messages;
using Janus.Communication.NetworkAdapters.Events;
using Janus.Communication.Remotes;
using Janus.Logging;
using Janus.Serialization;
using System.Net;
using System.Net.Sockets;

namespace Janus.Communication.NetworkAdapters.Tcp;

/// <summary>
/// Base class for a TCP network adapter
/// </summary>
public abstract class NetworkAdapter : INetworkAdapter
{
    protected readonly TcpListener _tcpListener;
    protected readonly int _listenPort;
    protected readonly CancellationTokenSource _cancellationTokenSource;
    protected Task? _listenerTask;
    protected IBytesSerializationProvider _serializationProvider;

    private readonly ILogger<NetworkAdapter>? _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="listenPort"></param>
    protected NetworkAdapter(int listenPort, IBytesSerializationProvider serializationProvider, ILogger? logger = null)
    {
        _logger = logger?.ResolveLogger<NetworkAdapter>();
        _serializationProvider = serializationProvider;
        _listenPort = listenPort;
        _tcpListener = new TcpListener(System.Net.IPAddress.Any, listenPort);
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public bool StartAdapter()
    {
        if (_listenerTask == null)
        {
            _listenerTask = Task.Run(() => RunListener(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
            _logger?.Info("Started tcp listener on {0}", _tcpListener.LocalEndpoint);
            return true;
        }
        return false;
    }

    public bool StopAdapter()
    {
        if (_listenerTask != null)
        {
            _cancellationTokenSource.Cancel();
            while (!_listenerTask.IsCanceled && !_listenerTask.IsFaulted && !_listenerTask.IsCompleted) ;
            _listenerTask?.Dispose();
            _listenerTask = null;
            _logger?.Info("Stopped tcp listener on {0}", _tcpListener.LocalEndpoint);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Runs a TCP listener that waits for incoming messages. This method is only top be run in a Task, due to while(true)
    /// </summary>
    /// <param name="cancellationToken"></param>
    private void RunListener(CancellationToken cancellationToken)
    {
        var listenerStarting = TryCatch(
            () => _tcpListener.Pass(l => l.Start()),
            ex => ex);

        if (listenerStarting.IsException)
        {
            _logger?.Error("Failed to start listener due to exception: {0}", listenerStarting.Exception);
        }
        else
        {
            _logger?.Info("Started tcp listener task on {0}", _tcpListener.LocalEndpoint);
        }


        while (true)
        {
            // maybe the task is cancelled?
            if (cancellationToken.IsCancellationRequested)
            {
                _logger?.Info("Tcp listening cancelled");
                cancellationToken.ThrowIfCancellationRequested();
            }

            // await a client
            var client = _tcpListener.AcceptTcpClientAsync(cancellationToken).Result;

            // if there wasn't a cancellation
            if (client != null)
            {
                _logger?.Info("Accepted client {0}", client.Client.RemoteEndPoint);
                // create a message bytes buffer
                byte[] messageBytes = new byte[0];
                // get stream from tcp client
                var stream = client.GetStream();
                // receive the bytes from the stream
                int readByte;
                while ((readByte = stream.ReadByte()) != -1)
                    messageBytes = messageBytes.Append((byte)readByte).ToArray();
                // .Read(messageBytes, 0, countBytes);
                // determine message type and raise event
                _serializationProvider.DetermineMessagePreamble(messageBytes)
                            .Bind<string, BaseMessage>(preamble => BuildMessage(preamble, messageBytes))
                            .Map(message => { RaiseMessageReceivedEvent(message, ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString()); return message; });
            }

        }
    }

    #region MESSAGE RECEIVED EVENTS
    /// <summary>
    /// Invoked when a HELLO_REQ message is received
    /// </summary>
    public event EventHandler<HelloReqReceivedEventArgs>? HelloRequestReceived;
    /// <summary>
    /// Invoked when a HELLO_RES message is received
    /// </summary>
    public event EventHandler<HelloResReceivedEventArgs>? HelloResponseReceived;
    /// <summary>
    /// Invoked when a BYE_REQ message is received
    /// </summary>
    public event EventHandler<ByeReqReceivedEventArgs>? ByeRequestReceived;
    #endregion

    #region SEND HELLO MESSAGES
    /// <summary>
    /// Sends a HELLO_REQ message to the remote point
    /// </summary>
    /// <param name="message">HELLO_REQ message</param>
    /// <param name="remotePoint">Target remote point</param>
    /// <returns></returns>
    public async Task<Result> SendHelloRequest(HelloReqMessage message, RemotePoint remotePoint)
        => await Task.FromResult(_serializationProvider.HelloReqMessageSerializer.Serialize(message))
            .Bind(async messageBytes => await SendMessageBytes(messageBytes, remotePoint)
                                            .Pass(result => _logger?.Info("Sending {0} to {1}", message.Preamble, remotePoint)));
    //=> await SendMessageBytes(message.ToBson(), remotePoint)
    //    .Pass(result => _logger?.Info("Sending {0} to {1}", message.Preamble, remotePoint));

    /// <summary>
    /// Sends a HELLO_RES message to the remote point
    /// </summary>
    /// <param name="message">HELLO_RES message</param>
    /// <param name="remotePoint">Target remote point</param>
    /// <returns></returns>
    public async Task<Result> SendHelloResponse(HelloResMessage message, RemotePoint remotePoint)
        => await Task.FromResult(_serializationProvider.HelloResMessageSerializer.Serialize(message))
            .Bind(async messageBytes => await SendMessageBytes(messageBytes, remotePoint)
                                            .Pass(result => _logger?.Info("Sending {0} to {1}", message.Preamble, remotePoint)));

    #endregion

    #region SEND BYE MESSAGES
    /// <summary>
    /// Sends a BYE_REQ message to the remote point
    /// </summary>
    /// <param name="message">BYE_REQ message</param>
    /// <param name="remotePoint">Target remote point</param>
    /// <returns></returns>
    public async Task<Result> SendByeRequest(ByeReqMessage message, RemotePoint remotePoint)
        => await Task.FromResult(_serializationProvider.ByeReqMessageSerializer.Serialize(message))
            .Bind(async messageBytes => await SendMessageBytes(messageBytes, remotePoint)
                                            .Pass(result => _logger?.Info("Sending {0} to {1}", message.Preamble, remotePoint)));
    #endregion

    /// <summary>
    /// Base method to raise message events
    /// </summary>
    /// <param name="message">Message instance</param>
    /// <param name="address">Address from which the message was received</param>
    private void RaiseMessageReceivedEvent(BaseMessage message, string address)
    {
        _logger?.Info("Received {0} from {1}", message.Preamble, address);
        switch (message.Preamble)
        {
            case Preambles.HELLO_REQUEST:
                HelloRequestReceived?.Invoke(this, new HelloReqReceivedEventArgs((HelloReqMessage)message, address));
                _logger?.Debug("Invoked HelloRequestReceived");
                break;
            case Preambles.HELLO_RESPONSE:
                HelloResponseReceived?.Invoke(this, new HelloResReceivedEventArgs((HelloResMessage)message, address));
                _logger?.Debug("Invoked HelloResponseReceived");
                break;
            case Preambles.BYE_REQUEST:
                ByeRequestReceived?.Invoke(this, new ByeReqReceivedEventArgs((ByeReqMessage)message, address));
                _logger?.Debug("Invoked ByeRequestReceived");
                break;
            default:
                RaiseSpecializedMessageReceivedEvent(message, address);
                break;
        }
    }

    /// <summary>
    /// Specialization method to raise message events for subclass node types
    /// </summary>
    /// <param name="message">Message instance</param>
    /// <param name="address">Address from which the message was received</param>
    public abstract void RaiseSpecializedMessageReceivedEvent(BaseMessage message, string address);

    /// <summary>
    /// Creates base messages from bytes read over the network
    /// </summary>
    /// <param name="preambule">Message preambule</param>
    /// <param name="messageBytes">Message bytes</param>
    /// <returns>Created message boxed as BaseMessage</returns>
    private Result<BaseMessage> BuildMessage(string preambule, byte[] messageBytes)
        => preambule switch
        {
            Preambles.HELLO_REQUEST => _serializationProvider.HelloReqMessageSerializer.Deserialize(messageBytes).Map(_ => (BaseMessage)_),
            Preambles.HELLO_RESPONSE => _serializationProvider.HelloResMessageSerializer.Deserialize(messageBytes).Map(_ => (BaseMessage)_),
            Preambles.BYE_REQUEST => _serializationProvider.ByeReqMessageSerializer.Deserialize(messageBytes).Map(_ => (BaseMessage)_),
            _ => BuildSpecializedMessage(preambule, messageBytes)
        };

    /// <summary>
    /// Creates node specialized messages from bytes read over the network
    /// </summary>
    /// <param name="preambule">Message preambule</param>
    /// <param name="messageBytes">Message bytes</param>
    /// <returns>Created message boxed as BaseMessage</returns>
    public abstract Result<BaseMessage> BuildSpecializedMessage(string preambule, byte[] messageBytes);

    /// <summary>
    /// Sends bytes of a message to a remote point over TCP
    /// </summary>
    /// <param name="messageBytes">Message serialized to bytes</param>
    /// <param name="remotePoint">Destination temote point</param>
    /// <returns>Result</returns>
    protected async Task<Result> SendMessageBytes(byte[] messageBytes, RemotePoint remotePoint)
        => await ResultExtensions.AsResult(
            () =>
                Using(() => new TcpClient(remotePoint.Address.ToString(), (int)remotePoint.Port),
                      async tcpClient =>
                        tcpClient.Connected && await tcpClient.Client.SendAsync(messageBytes, SocketFlags.None) == messageBytes.Length
                    )
            );

    public void Dispose()
    {
        StopAdapter();
        _cancellationTokenSource.Dispose();
        _tcpListener.Stop();
    }
}
