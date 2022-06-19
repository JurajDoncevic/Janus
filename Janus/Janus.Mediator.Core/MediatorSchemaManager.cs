using Janus.Commons.SchemaModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Components.Core;

namespace Janus.Mediator.Core;
public class MediatorSchemaManager : IComponentSchemaManager
{
    private readonly MediatorCommunicationNode _communicationNode;

    public MediatorSchemaManager(MediatorCommunicationNode communicationNode)
    {
        _communicationNode = communicationNode;
    }
    public Result<DataSource> GetCurrentSchema()
    {
        throw new NotImplementedException();
    }

    public Result<DataSource> GetSchemaFromNode(string nodeId)
    {
        throw new NotImplementedException();
    }

    public Result<DataSource> ReloadSchema(object transformations = null)
    {
        throw new NotImplementedException();
    }
}
