using Janus.Base;
using Janus.Base.Resulting;
using Janus.Commons.SchemaModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Logging;
using Janus.Mask.Persistence;
using Janus.Mask.Sqlite.MaskedCommandModel;
using Janus.Mask.Sqlite.MaskedDataModel;
using Janus.Mask.Sqlite.MaskedQueryModel;
using Janus.Mask.Sqlite.MaskedSchemaModel;
using Janus.Mask.Sqlite.Materialization;

namespace Janus.Mask.Sqlite;
public sealed class SqliteMaskManager : MaskManager<SqliteQuery, TableauId, Unit, Unit, Unit, SqliteDelete, SqliteInsert, SqliteUpdate, Unit, Unit, Database, SqliteTabularData, SqliteDataRow>
{
    private readonly SqliteMaskQueryManager _queryManager;
    private readonly SqliteMaskCommandManager _commandManager;
    private readonly SqliteMaskSchemaManager _schemaManager;
    private readonly ILogger<SqliteMaskManager>? _logger;
    private readonly DatabaseMaterializer _databaseMaterializer;
    private readonly SqliteMaskOptions _maskOptions;

    public SqliteMaskManager(MaskCommunicationNode communicationNode, SqliteMaskQueryManager queryManager, SqliteMaskCommandManager commandManager, SqliteMaskSchemaManager schemaManager, MaskPersistenceProvider persistenceProvider, SqliteMaskOptions maskOptions, ILogger? logger = null) : base(communicationNode, queryManager, commandManager, schemaManager, persistenceProvider, maskOptions, logger)
    {
        _queryManager = queryManager;
        _commandManager = commandManager;
        _schemaManager = schemaManager;
        _logger = logger?.ResolveLogger<SqliteMaskManager>();
        _databaseMaterializer = new DatabaseMaterializer();
        _maskOptions = maskOptions;

        if (_maskOptions.EagerStartup)
        {
            if (_maskOptions.StartupMaterializeDatabase)
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
            if (!_schemaManager.CurrentOutputSchema)
            {
                return Results.OnFailure("No schema currently loaded!");
            }

            if (!_schemaManager.CurrentMaskedSchema)
            {
                return Results.OnFailure("No masked schema currently generated!");
            }

            return await _databaseMaterializer.MaterializeDatabase(_maskOptions.MaterializationConnectionString, _schemaManager.CurrentMaskedSchema.Value, _schemaManager.CurrentOutputSchema.Value, _queryManager);
        })).Pass(
            r => _logger?.Info($"Materialized database successfully: {r.Message}"),
            r => _logger?.Info($"Failed database materialization: {r.Message}")
            );
}
