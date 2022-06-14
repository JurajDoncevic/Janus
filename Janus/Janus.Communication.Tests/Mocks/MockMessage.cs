using Janus.Communication.Messages;

namespace Janus.Communication.Tests.Mocks;

internal class MockMessage : BaseMessage
{
    internal MockMessage(string exchangeId, string nodeId, string preamble) : base(exchangeId, nodeId, preamble)
    {
    }

    internal MockMessage(string nodeId, string preamble) : base(nodeId, preamble)
    {
    }
}

internal static class MockMesagePreambles
{
    internal static string MOCK_REQ = "MOCK_REQ";
    internal static string MOCK_RES = "MOCK_RES";
}
