using Janus.Communication.Messages;

namespace Janus.Communication.NetworkAdapters.Events;

public class BaseMessageReceivedEventArgs<TMessage> where TMessage : BaseMessage
{
    private readonly TMessage _message;
    private readonly string _senderAddress;
    public TMessage Message => _message;
    public string SenderAddress => _senderAddress;

    public BaseMessageReceivedEventArgs(TMessage message, string senderAddress)
    {
        _message = message;
        _senderAddress = senderAddress;
    }
}