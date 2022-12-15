using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.SchemaModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Communication.Remotes;
using Janus.Components;
using Janus.Logging;
using Janus.Mediation;
using Janus.Mediation.SchemaMediationModels;
using System.Runtime.InteropServices;

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

    /// <summary>
    /// Currently loaded data source schemas by their name
    /// </summary>
    public IReadOnlyDictionary<string, DataSource> LoadedDataSourceWithName => _loadedDataSources.ToDictionary(kv => kv.Key, kv => kv.Value.dataSource);
    /// <summary>
    /// Remote points of the currently loaded data source schemas by data source name
    /// </summary>
    public IReadOnlyDictionary<string, RemotePoint> RemotePointWithLoadedDataSourceName => _loadedDataSources.ToDictionary(kv => kv.Key, kv => kv.Value.remotePoint);
    /// <summary>
    /// Currently loaded data source schemas by their remote point
    /// </summary>
    public IReadOnlyDictionary<RemotePoint, DataSource> LoadedDataSourceFromRemotePoint => _loadedDataSources.Values.ToDictionary(kv => kv.remotePoint, kv => kv.dataSource);
    /// <summary>
    /// Currently used mediation
    /// </summary>
    public Option<DataSourceMediation> CurrentMediation => _currentMediation;
    /// <summary>
    /// Current mediated schema
    /// </summary>
    public Option<DataSource> CurrentMediatedSchema => _currentMediatedSchema;
    /// <summary>
    /// List of currently loaded data sources
    /// </summary>
    public IReadOnlyList<DataSource> LoadedDataSources => _loadedDataSources.Values.Map(v => v.dataSource).ToList();


    public Result UnloadSchema(string dataSourceName)
        => Option<RemotePoint>.Some(RemotePointWithLoadedDataSourceName[dataSourceName])
            .Match(
            rp => UnloadSchema(rp),
            () => Results.OnFailure($"Data source schema with name {dataSourceName} not loaded.")
            );
    public Result UnloadSchema(RemotePoint remotePoint)
        => Results.AsResult(() =>
        {
            if (!LoadedDataSourceFromRemotePoint.ContainsKey(remotePoint))
            {
                return Results.OnFailure($"No loaded schema from remote point {remotePoint}.");
            }

            string dataSourceName = LoadedDataSourceFromRemotePoint[remotePoint].Name;

            return _loadedDataSources.Remove(dataSourceName)
                ? Results.OnSuccess()
                : Results.OnFailure($"Failed to remove {remotePoint} from loaded data sources.");
        });

    public Result UnloadSchemas()
        => Results.AsResult(() => { _loadedDataSources.Clear(); return true; });

    public async Task<Result<DataSource>> LoadSchema(RemotePoint remotePoint)
        => await Results.AsResult(async () =>
        {
            if (remotePoint.RemotePointType == RemotePointTypes.UNDETERMINED || remotePoint.RemotePointType == RemotePointTypes.MASK)
            {
                _logger?.Info($"Refusing to include schema of remote point {remotePoint} to loaded schemas due to inapropriate type");
                return Results.OnFailure<DataSource>("Can't load schema from MASK or UNDETERMINED component type.");
            }

            var schemaReqResult = await _communicationNode.SendSchemaRequest(remotePoint);
            if (schemaReqResult)
            {
                _loadedDataSources.Add(schemaReqResult.Data.Name, (remotePoint, schemaReqResult.Data));
            }

            return schemaReqResult;
        });

    public async Task<Result<DataSource>> GetSchemaFrom(RemotePoint remotePoint)
        => await _communicationNode.SendSchemaRequest(remotePoint);

    public async Task<Dictionary<RemotePoint, Result<DataSource>>> GetSchemasFromComponents()
        => (await Task.WhenAll(
            _communicationNode.RemotePoints
                .Map(async rp => (remotePoint: rp, schemaResult: await _communicationNode.SendSchemaRequest(rp)))  
            )).ToDictionary(kv => kv.remotePoint, kv => kv.schemaResult);

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