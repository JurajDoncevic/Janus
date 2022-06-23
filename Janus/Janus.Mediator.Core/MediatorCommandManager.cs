using Janus.Commons.CommandModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Components;

namespace Janus.Mediator.Core;
public class MediatorCommandManager : IComponentCommandManager, IDelegatingCommandManager
{
    private readonly MediatorCommunicationNode _communicationNode;

    public MediatorCommandManager(MediatorCommunicationNode communicationNode)
    {
        _communicationNode = communicationNode;
    }

    public Task<Result> RunCommand(BaseCommand command)
    {
        throw new NotImplementedException();
    }

    public Task<Result> RunCommandOnComponent(BaseCommand command, string nodeId)
    {
        throw new NotImplementedException();
    }
}
