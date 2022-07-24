using Janus.Serialization;
using Janus.Serialization.MongoBson;

namespace Janus.Communication.Tests.TestFixtures;

public class MongoBsonCommunicationNodeTestFixture : CommunicationNodeTestFixture
{
    public override IBytesSerializationProvider SerializationProvider => new MongoBsonSerializationProvider();
}
