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

    private DataSource? _currentOutputSchema;

    public IReadOnlyList<RemotePoint> SchemaInferredRemotePoints => _schemaInferredRemotePoints.ToList();

    public MediatorSchemaManager(MediatorCommunicationNode communicationNode, IEnumerable<RemotePoint>? schemaIncludedRemotePoints = null)
    {
        _communicationNode = communicationNode;
        _schemaInferredRemotePoints = schemaIncludedRemotePoints?.ToHashSet() ?? new HashSet<RemotePoint>();
    }

    public Result AddRemotePointToSchemaInferrence(RemotePoint remotePoint)
    {
        // get the remote point from the communication node via node id
        // cannot be a MASK or UNKNOWN
        var registeredRemotePoint = _communicationNode.RemotePoints
                            .FirstOrDefault(rp => rp.Equals(remotePoint) && 
                                                  rp.RemotePointType != RemotePointTypes.MASK && 
                                                  rp.RemotePointType != RemotePointTypes.UNDETERMINED);
        if(registeredRemotePoint == null)
        {
            return Result.OnFailure($"Remote point {remotePoint} isn't registered or is of incorrect type");
        }    

        return _schemaInferredRemotePoints.Add(registeredRemotePoint)
            ? Result.OnSuccess($"Added {registeredRemotePoint} to schema inferrence")
            : Result.OnFailure($"Remote point {registeredRemotePoint} already in schema inferrence");
    }

    public Result RemoveRemotePointFromSchemaInferrence(RemotePoint remotePoint)
    {
        var inferredRemotePoint = _schemaInferredRemotePoints.FirstOrDefault(rp => rp.Equals(remotePoint));
        if(inferredRemotePoint == null)
        {
            return Result.OnFailure($"Remote point {remotePoint} not in schema inferrence");
        }

        return _schemaInferredRemotePoints.Remove(inferredRemotePoint)
            ? Result.OnSuccess($"Removed {inferredRemotePoint} from schema inferrence")
            : Result.OnFailure($"Failed to remove {inferredRemotePoint} from schema inferrence");
    }

    public async Task<Result<DataSource>> GetCurrentOutputSchema()
        => await (_currentOutputSchema != null
            ? Task.FromResult(Result<DataSource>.OnSuccess(_currentOutputSchema))
            : Task.FromResult(Result<DataSource>.OnFailure("No schema loaded")));

    public async Task<Result<DataSource>> GetSchemaFromRemotePoint(RemotePoint remotePoint)
    {
        var inferredRemotePoint = _schemaInferredRemotePoints.FirstOrDefault(rp => rp.Equals(remotePoint));
        if (remotePoint == null)
        {
            return Result<DataSource>.OnFailure($"Remote point {remotePoint} not in schema inferrence");
        }

        return await _communicationNode.SendSchemaRequest(remotePoint);
    }

    public async Task<IEnumerable<Result<DataSource>>> GetInputSchemata()
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

    public Task<Result<DataSource>> ReloadOutputSchema()
    {
        throw new NotImplementedException();
    }
}
