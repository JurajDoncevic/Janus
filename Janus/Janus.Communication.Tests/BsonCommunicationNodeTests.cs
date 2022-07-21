using Janus.Communication.Tests.TestFixtures;

namespace Janus.Communication.Tests;
public class BsonCommunicationNodeTests : CommunicationNodeTests<BsonCommunicationNodeTestFixture>
{
    public BsonCommunicationNodeTests(BsonCommunicationNodeTestFixture testFixture) : base(testFixture)
    {
    }
}
