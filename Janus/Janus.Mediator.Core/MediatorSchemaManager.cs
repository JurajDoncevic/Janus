using Janus.Commons.SchemaModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Components;

namespace Janus.Mediator.Core;
public class MediatorSchemaManager : IComponentSchemaManager, IDelegatingSchemaManager
{
    private readonly MediatorCommunicationNode _communicationNode;

    public MediatorSchemaManager(MediatorCommunicationNode communicationNode)
    {
        _communicationNode = communicationNode;
    }
    public Task<Result<DataSource>> GetCurrentSchema()
    {
        throw new NotImplementedException();
    }

    public Task<Result<DataSource>> GetSchemaFromComponent(string nodeId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<DataSource>> ReloadSchema(object transformations = null)
    {
        throw new NotImplementedException();
    }
}
