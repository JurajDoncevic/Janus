using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.SchemaModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Communication.Remotes;
using Janus.Components;
using Janus.Logging;
using Janus.Mediation.SchemaMediationModels;

namespace Janus.Mediator;
public sealed class MediatorSchemaManager : IComponentSchemaManager, IMediatingSchemaManager, IDelegatingSchemaManager
{
    private readonly MediatorCommunicationNode _communicationNode;

    private Option<DataSource> _currentSchema;
    private readonly ILogger<MediatorSchemaManager>? _logger;
    private Option<DataSourceMediation> _currentMediation;
    private readonly Dictionary<RemotePoint, DataSource> _currentDataSourcesOnRemotePoints;

    public IReadOnlyDictionary<string, DataSource> CurrentDataSourcesSchemas => _currentDataSourcesOnRemotePoints.ToDictionary(kv => kv.Value.Name, kv => kv.Value);

    public MediatorSchemaManager(MediatorCommunicationNode communicationNode, ILogger? logger = null)
    {
        _communicationNode = communicationNode;
        _currentSchema = Option<DataSource>.None;
        _currentMediation= Option<DataSourceMediation>.None;
        _currentDataSourcesOnRemotePoints= new Dictionary<RemotePoint, DataSource>();

        _logger = logger?.ResolveLogger<MediatorSchemaManager>();
    }

    public Option<DataSource> GetCurrentOutputSchema()
        => _currentSchema;

    public async Task<Result<DataSource>> ReloadOutputSchema()
        => _currentMediation
            ? await GenerateMediatedSchema(_currentMediation.Value)
            : await Task.FromResult(Results.OnFailure<DataSource>("No mediation set to reload a mediator schema"));

    public async Task<IEnumerable<Result<DataSource>>> GetSchemasFromComponents()
        => await Task.WhenAll(
            _communicationNode.RemotePoints
                              .Map(rp => _communicationNode.SendSchemaRequest(rp))
            );

    public async Task<Result<DataSource>> GetDataSourceSchemaFrom(RemotePoint remotePoint)
        => remotePoint.RemotePointType != RemotePointTypes.MASK || remotePoint.RemotePointType != RemotePointTypes.UNDETERMINED
            ? await _communicationNode.SendSchemaRequest(remotePoint)
            : await Task.FromResult(Results.OnFailure<DataSource>($"Given remote point {remotePoint} is not of valid type for mediator schema requests."));

    public async Task<Result<DataSource>> GenerateMediatedSchema(DataSourceMediation mediation)
        => !_currentDataSourcesOnRemotePoints.Values.SequenceEqual(mediation.AvailableDataSources.Values)
            ? await Task.FromResult(Results.OnFailure<DataSource>("Current data sources don't align with the ones in the mediation"))
            : (await Task.FromResult(Mediation.SchemaModelMediation.MediateDataSource(mediation)))
                .Pass(
                    r => { _currentSchema = Option<DataSource>.Some(r.Data); _currentMediation = Option<DataSourceMediation>.Some(mediation); }
                );

    public async Task<IEnumerable<Result<DataSource>>> ReloadSchemasFromComponents()
        => (await Task.WhenAll(
            _communicationNode.RemotePoints
                              .Map(async rp => (remotePoint: rp, result: await _communicationNode.SendSchemaRequest(rp)))
            ))
            .Pass(_ => _currentDataSourcesOnRemotePoints.Clear())
            .Map(tuple => tuple.result.Pass(r => _currentDataSourcesOnRemotePoints.Add(tuple.remotePoint, r.Data)));
}