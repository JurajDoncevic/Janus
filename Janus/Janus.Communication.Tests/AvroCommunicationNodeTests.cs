using Janus.Communication.Tests.TestFixtures;
using Xunit;

namespace Janus.Communication.Tests;

[Collection("Communication Node Tests")]
public class AvroCommunicationNodeTests : CommunicationNodeTests<AvroCommunicationNodeTestFixture>
{
    public AvroCommunicationNodeTests(AvroCommunicationNodeTestFixture testFixture) : base(testFixture)
    {
    }
}
