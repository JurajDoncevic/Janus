using Janus.Communication.Messages;

namespace Janus.Communication.NetworkAdapters;

public class HelloResponseReceivedEventArgs : EventArgs
{
    private readonly HelloResMessage _message;
    private readonly string _senderAddress;
    public HelloResMessage Message => _message;
    public string SenderAddress => _senderAddress;

    public HelloResponseReceivedEventArgs(HelloResMessage message, string senderAddress) : base()
    {
        _message = message;
        _senderAddress = senderAddress;
    }
}

