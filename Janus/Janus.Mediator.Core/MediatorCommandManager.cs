using Janus.Commons.CommandModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Components.Core;

namespace Janus.Mediator.Core;
public class MediatorCommandManager : IComponentCommandManager
{
    private readonly MediatorCommunicationNode _communicationNode;

    public MediatorCommandManager(MediatorCommunicationNode communicationNode)
    {
        _communicationNode = communicationNode;
    }

    public Task<Result> ExecuteCommandGlobally(BaseCommand command)
    {
        throw new NotImplementedException();
    }

    public Task<Result> ExecuteCommandOnNode(BaseCommand command, string nodeId)
    {
        throw new NotImplementedException();
    }
}
