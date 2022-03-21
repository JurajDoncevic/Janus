using Janus.Communication.Messages;

namespace Janus.Communication.NetworkAdapters;

public class HelloReceivedEventArgs : EventArgs
{
    private readonly HelloReqMessage _message;
    private readonly string _senderAddress;
    public HelloReqMessage Message => _message;
    public string SenderAddress => _senderAddress;

    public HelloReceivedEventArgs(HelloReqMessage message, string senderAddress) : base()
    {
        _message = message;
        _senderAddress = senderAddress;
    }
}

