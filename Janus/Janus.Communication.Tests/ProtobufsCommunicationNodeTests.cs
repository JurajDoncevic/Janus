using Janus.Communication.Tests.TestFixtures;
using Xunit;

namespace Janus.Communication.Tests;

[Collection("Communication Node Tests")]
public class ProtobufsCommunicationNodeTests : CommunicationNodeTests<ProtobufsCommunicationNodeTestFixture>
{
    public ProtobufsCommunicationNodeTests(ProtobufsCommunicationNodeTestFixture testFixture) : base(testFixture)
    {
    }
}
