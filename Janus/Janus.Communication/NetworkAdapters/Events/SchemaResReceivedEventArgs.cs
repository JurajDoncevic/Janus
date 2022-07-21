using Janus.Commons.Messages;

namespace Janus.Communication.NetworkAdapters.Events;

public class SchemaResReceivedEventArgs : BaseMessageReceivedEventArgs<SchemaResMessage>
{
    public SchemaResReceivedEventArgs(SchemaResMessage message, string senderAddress) : base(message, senderAddress)
    {
    }
}
