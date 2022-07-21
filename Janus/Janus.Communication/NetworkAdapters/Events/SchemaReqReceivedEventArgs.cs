using Janus.Commons.Messages;

namespace Janus.Communication.NetworkAdapters.Events;

public class SchemaReqReceivedEventArgs : BaseMessageReceivedEventArgs<SchemaReqMessage>
{
    public SchemaReqReceivedEventArgs(SchemaReqMessage message, string senderAddress) : base(message, senderAddress)
    {
    }
}
