using Janus.Serialization;
using Janus.Serialization.Protobufs;

namespace Janus.Communication.Tests.TestFixtures;

public class ProtobufsCommunicationNodeTestFixture : CommunicationNodeTestFixture
{
    public override IBytesSerializationProvider SerializationProvider => new ProtobufsSerializationProvider();
}
