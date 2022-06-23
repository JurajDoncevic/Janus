using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Components;

namespace Janus.Mediator.Core;
public class MediatorQueryManager : IComponentQueryManager
{
    private readonly MediatorCommunicationNode _communicationNode;

    public MediatorQueryManager(MediatorCommunicationNode communicationNode)
    {
        _communicationNode = communicationNode;
    }

    public Task<Result<TabularData>> ExecuteQueryGlobally(Query query)
    {
        throw new NotImplementedException();
    }

    public Task<Result<TabularData>> ExecuteQueryOnNode(Query query, string nodeId)
    {
        throw new NotImplementedException();
    }
}
