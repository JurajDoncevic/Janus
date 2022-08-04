using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Results;
using Janus.Commons.SchemaModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Components;

namespace Janus.Mediator;
public sealed class MediatorSchemaManager : IComponentSchemaManager, ITransformingSchemaManager
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

    public Task<Result<DataSource>> ReloadSchema(object? transformations = null)
    {
        throw new NotImplementedException();
    }

    public Task<Result<DataSource>> ReloadSchema()
    {
        throw new NotImplementedException();
    }
}
