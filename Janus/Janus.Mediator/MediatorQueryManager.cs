using FunctionalExtensions.Base.Results;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Components;

namespace Janus.Mediator;
public sealed class MediatorQueryManager : IComponentQueryManager, IDelegatingQueryManager
{
    private readonly MediatorCommunicationNode _communicationNode;

    public MediatorQueryManager(MediatorCommunicationNode communicationNode)
    {
        _communicationNode = communicationNode;
    }

    public Task<Result<TabularData>> RunQuery(Query query)
    {
        throw new NotImplementedException();
    }

    public Task<Result<TabularData>> RunQueryOnComponent(Query query, string nodeId)
    {
        throw new NotImplementedException();
    }
}
