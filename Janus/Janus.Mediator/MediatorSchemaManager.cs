using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Results;
using Janus.Commons.SchemaModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Communication.Remotes;
using Janus.Components;

namespace Janus.Mediator;
public sealed class MediatorSchemaManager : IComponentSchemaManager, ITransformingSchemaManager
{
    private readonly MediatorCommunicationNode _communicationNode;

    private HashSet<RemotePoint> _schemaInferredRemotePoints;

    private DataSource? _currentSchema;

    public IReadOnlyList<RemotePoint> SchemaInferredRemotePoints => _schemaInferredRemotePoints.ToList();

    public MediatorSchemaManager(MediatorCommunicationNode communicationNode, IEnumerable<RemotePoint>? schemaIncludedRemotePoints = null)
    {
        _communicationNode = communicationNode;
        _schemaInferredRemotePoints = schemaIncludedRemotePoints?.ToHashSet() ?? new HashSet<RemotePoint>();
    }

    public Result AddRemotePointToSchemaInferrence(string nodeId)
    {
        // get the remote point from the communication node via node id
        // cannot be a MASK or UNKNOWN
        var remotePoint = _communicationNode.RemotePoints
                            .FirstOrDefault(rp => rp.NodeId.Equals(nodeId) && 
                                                  rp.RemotePointType != RemotePointTypes.MASK && 
                                                  rp.RemotePointType != RemotePointTypes.UNDETERMINED);
        if(remotePoint == null)
        {
            return Result.OnFailure($"Remote point with node id {nodeId} isn't registered or is of incorrect type");
        }    

        return _schemaInferredRemotePoints.Add(remotePoint)
            ? Result.OnSuccess($"Added {remotePoint} to schema inferrence")
            : Result.OnFailure($"Remote point {remotePoint} already in schema inferrence");
    }

    public Result RemoveRemotePointFromSchemaInferrence(string nodeId)
    {
        var remotePoint = _schemaInferredRemotePoints.FirstOrDefault(rp => rp.Equals(nodeId));
        if(remotePoint == null)
        {
            return Result.OnFailure($"Remote point with node id {nodeId} not in schema inferrence");
        }

        return _schemaInferredRemotePoints.Remove(remotePoint)
            ? Result.OnSuccess($"Removed {remotePoint} from schema inferrence")
            : Result.OnFailure($"Failed to remove {remotePoint} from schema inferrence");
    }

    public async Task<Result<DataSource>> GetCurrentSchema()
        => await (_currentSchema != null
            ? Task.FromResult(Result<DataSource>.OnSuccess(_currentSchema))
            : Task.FromResult(Result<DataSource>.OnFailure("No schema loaded")));

    public async Task<Result<DataSource>> GetSchemaFromComponent(string nodeId)
    {
        var remotePoint = _schemaInferredRemotePoints.FirstOrDefault(rp => rp.Equals(nodeId));
        if (remotePoint == null)
        {
            return Result<DataSource>.OnFailure($"Remote point with node id {nodeId} not in schema inferrence");
        }

        return await _communicationNode.SendSchemaRequest(remotePoint);
    }

    public async Task<IEnumerable<Result<DataSource>>> GetAllSchemataFromComponents()
    {
        var schemaRequestTasks =
        _schemaInferredRemotePoints
            .Select(_communicationNode.SendSchemaRequest);
            

        return await Task.WhenAll(schemaRequestTasks);
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
