using Janus.Communication.Tests.TestFixtures;
using Xunit;

namespace Janus.Communication.Tests;

[Collection("Communication Node Tests")]
public class BsonCommunicationNodeTests : CommunicationNodeTests<BsonCommunicationNodeTestFixture>
{
    public BsonCommunicationNodeTests(BsonCommunicationNodeTestFixture testFixture) : base(testFixture)
    {
    }
}
