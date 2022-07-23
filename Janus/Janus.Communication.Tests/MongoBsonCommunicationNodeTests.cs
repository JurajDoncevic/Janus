using Janus.Communication.Tests.TestFixtures;
using Xunit;

namespace Janus.Communication.Tests;

[Collection("Communication Node Tests")]
public class MongoBsonCommunicationNodeTests : CommunicationNodeTests<MongoBsonCommunicationNodeTestFixture>
{
    public MongoBsonCommunicationNodeTests(MongoBsonCommunicationNodeTestFixture testFixture) : base(testFixture)
    {
    }
}
