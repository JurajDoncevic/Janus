using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.SchemaModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Communication.Remotes;
using Janus.Components;
using Janus.Logging;
using Janus.Mediation;
using Janus.Mediation.SchemaMediationModels;

namespace Janus.Mediator;
public sealed class MediatorSchemaManager : IComponentSchemaManager, IDelegatingSchemaManager, IMediatingSchemaManager
{
    private readonly MediatorCommunicationNode _communicationNode;
    private readonly Dictionary<string, (RemotePoint remotePoint, DataSource dataSource)> _loadedDataSources;
    private readonly ILogger<MediatorSchemaManager>? _logger;
    private Option<DataSourceMediation> _currentMediation;
    private Option<DataSource> _currentMediatedSchema;

    public MediatorSchemaManager(MediatorCommunicationNode communicationNode, ILogger? logger = null)
    {
        _communicationNode = communicationNode;
        _loadedDataSources = new Dictionary<string, (RemotePoint remotePoint, DataSource dataSource)>();
        _currentMediation = Option<DataSourceMediation>.None;
        _currentMediatedSchema = Option<DataSource>.None;
        _logger = logger?.ResolveLogger<MediatorSchemaManager>();
    }

    public IReadOnlyDictionary<string, DataSource> LoadedDataSourceWithName => _loadedDataSources.ToDictionary(kv => kv.Key, kv => kv.Value.dataSource);
    public IReadOnlyDictionary<string, RemotePoint> RemotePointWithLoadedDataSourceName => _loadedDataSources.ToDictionary(kv => kv.Key, kv => kv.Value.remotePoint);
    public IReadOnlyDictionary<RemotePoint, DataSource> DataSourceFromRemotePoint => _loadedDataSources.Values.ToDictionary(kv => kv.remotePoint, kv => kv.dataSource);

    public Result ExcludeFromLoadedSchemas(RemotePoint remotePoint)
        => Results.AsResult(() =>
        {
            if (!DataSourceFromRemotePoint.ContainsKey(remotePoint))
            {
                return Results.OnFailure($"No loaded schema from remote point {remotePoint}.");
            }

            string dataSourceName = DataSourceFromRemotePoint[remotePoint].Name;

            return _loadedDataSources.Remove(dataSourceName)
                ? Results.OnSuccess()
                : Results.OnFailure($"Failed to remove {remotePoint} from loaded data sources.");
        });

    public async Task<Result<DataSource>> IncludeInLoadedSchemas(RemotePoint remotePoint)
        => await Results.AsResult(async () =>
        {
            if (remotePoint.RemotePointType == RemotePointTypes.UNDETERMINED || remotePoint.RemotePointType == RemotePointTypes.MASK)
            {
                _logger?.Info($"Refusing to include schema of remote point {remotePoint} to loaded schemas due to inapropriate type");
                return Results.OnFailure<DataSource>("Can't load schema from MASK or UNDETERMINED component type.");
            }

            return await _communicationNode.SendSchemaRequest(remotePoint);
        });

    public async Task<Result<DataSource>> GetSchemaFrom(RemotePoint remotePoint)
        => await _communicationNode.SendSchemaRequest(remotePoint);

    public async Task<IEnumerable<Result<DataSource>>> GetSchemasFromComponents()
        => await Task.WhenAll(
            _communicationNode.RemotePoints
                .Map(rp => _communicationNode.SendSchemaRequest(rp))
            );

    public async Task<IEnumerable<Result<DataSource>>> ReloadSchemas()
        => await Task.WhenAll(
            _loadedDataSources.Values.Map(t => t.remotePoint)
            .Pass(_ => _loadedDataSources.Clear())
            .Map(async remotePoint => await Results.AsResult(async () =>
            {
                var resultForRemotePoint = await _communicationNode.SendSchemaRequest(remotePoint);
                if (resultForRemotePoint)
                {
                    _loadedDataSources.Add(resultForRemotePoint.Data.Name, (remotePoint, resultForRemotePoint.Data));
                    _logger?.Info($"Loaded schema {resultForRemotePoint.Data.Name} from remote point {remotePoint}");
                }
                else
                {
                    _logger?.Info($"Failed to request schema from {remotePoint}");
                }
                return resultForRemotePoint;
            }))
            );

    public async Task<Result<DataSource>> MediateLoadedSchemas(DataSourceMediation mediation)
        => await Task.FromResult(Results.AsResult(() =>
        {
            if (!mediation.AvailableDataSources.SequenceEqual(LoadedDataSourceWithName))
            {
                return Results.OnFailure<DataSource>($"Loaded data sources not the same as the ones referenced in the mediation.");
            }

            var mediatedDataSourceResult = SchemaModelMediation.MediateDataSource(mediation);
            if (mediatedDataSourceResult)
            {
                _currentMediation = Option<DataSourceMediation>.Some(mediation);
                _currentMediatedSchema = Option<DataSource>.Some(mediatedDataSourceResult.Data);
            }
            return mediatedDataSourceResult;
        }));

    public Option<DataSource> GetCurrentOutputSchema()
        => _currentMediatedSchema;

    public async Task<Result<DataSource>> ReloadOutputSchema()
        => await _currentMediation.Match(
            async currentMediation => await (await ReloadSchemas())
                                        .Identity()
                                        .Map(_ => MediateLoadedSchemas(currentMediation)).Data,
            async () => await Task.FromResult(Results.OnFailure<DataSource>("No mediation currently given"))
            );
}