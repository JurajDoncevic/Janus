using Janus.Base;
using Janus.Base.Resulting;
using Janus.Commons.SchemaModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Logging;
using Janus.Mask.LiteDB.MaskedCommandModel;
using Janus.Mask.LiteDB.MaskedDataModel;
using Janus.Mask.LiteDB.MaskedQueryModel;
using Janus.Mask.LiteDB.MaskedSchemaModel;
using Janus.Mask.LiteDB.Materialization;
using Janus.Mask.Persistence;
using LiteDB;

namespace Janus.Mask.LiteDB;
public sealed class LiteDbMaskManager : MaskManager<LiteDbQuery, TableauId, Unit, Unit, Unit, LiteDbDelete, LiteDbInsert, LiteDbUpdate, Unit, Unit, Database, LiteDbData, BsonDocument>
{
    private readonly LiteDbMaskQueryManager _queryManager;
    private readonly LiteDbMaskCommandManager _commandManager;
    private readonly LiteDbMaskSchemaManager _schemaManager;
    private readonly ILogger<LiteDbMaskManager>? _logger;
    private readonly DatabaseMaterializer _databaseMaterializer;
    private readonly LiteDbMaskOptions _maskOptions;

    public LiteDbMaskManager(MaskCommunicationNode communicationNode, LiteDbMaskQueryManager queryManager, LiteDbMaskCommandManager commandManager, LiteDbMaskSchemaManager schemaManager, MaskPersistenceProvider persistenceProvider, LiteDbMaskOptions maskOptions, ILogger? logger = null) : base(communicationNode, queryManager, commandManager, schemaManager, persistenceProvider, maskOptions, logger)
    {
        _queryManager = queryManager;
        _commandManager = commandManager;
        _schemaManager = schemaManager;
        _maskOptions = maskOptions;
        _logger = logger?.ResolveLogger<LiteDbMaskManager>();

        _databaseMaterializer = new DatabaseMaterializer();

        if (_maskOptions.EagerStartup)
        {
            if(_maskOptions.StartupMaterializeDatabase)
            {
                StartupMaterializeDatabase().GetAwaiter().GetResult();
            }
        }
    }

    private async Task<Result> StartupMaterializeDatabase()
        => await MaterializeDatabase(_maskOptions.MaterializationConnectionString);

    public async Task<Result> MaterializeDatabase(string? connectionString = null)
        => (await Results.AsResult(async () =>
        {
            if(!_schemaManager.CurrentOutputSchema)
            {
                return Results.OnFailure("No schema currently loaded!");
            }

            if (!_schemaManager.CurrentMaskedSchema)
            {
                return Results.OnFailure("No masked schema currently generated!");
            }

            connectionString ??= _maskOptions.MaterializationConnectionString;

            using var liteDb = new LiteDatabase(connectionString);

            return await _databaseMaterializer.MaterializeDatabase(liteDb, _schemaManager.CurrentMaskedSchema.Value, _schemaManager.CurrentOutputSchema.Value, _queryManager);
        })).Pass(
            r => _logger?.Info($"Materialized database successfully: {r.Message}"),
            r => _logger?.Info($"Failed database materialization: {r.Message}")
            );
}
