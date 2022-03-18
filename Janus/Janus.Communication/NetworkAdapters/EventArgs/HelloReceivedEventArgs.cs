using Janus.Communication.Messages;

namespace Janus.Communication.NetworkAdapters;

public class HelloReceivedEventArgs : EventArgs
{
    private readonly HelloMessage _message;
    private readonly string _senderAddress;
    public HelloMessage Message => _message;
    public string SenderAddress => _senderAddress;

    public HelloReceivedEventArgs(HelloMessage message, string senderAddress) : base()
    {
        _message = message;
        _senderAddress = senderAddress;
    }
}

