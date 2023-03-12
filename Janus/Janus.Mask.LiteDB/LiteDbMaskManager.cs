using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.SchemaModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Logging;
using Janus.Mask.LiteDB.MaskedCommandModel;
using Janus.Mask.LiteDB.MaskedQueryModel;
using Janus.Mask.LiteDB.MaskedSchemaModel;
using Janus.Mask.LiteDB.Materialization;
using Janus.Mask.Persistence;
using LiteDB;

namespace Janus.Mask.LiteDB;
public sealed class LiteDbMaskManager : MaskManager<LiteDbQuery, TableauId, Unit, Unit, Unit, LiteDbDelete, LiteDbInsert, LiteDbUpdate, Unit, Unit, Database>
{
    private readonly LiteDbMaskQueryManager _queryManager;
    private readonly LiteDbMaskSchemaManager _schemaManager;
    private readonly ILogger<LiteDbMaskManager>? _logger;
    private readonly DatabaseMaterializer _databaseMaterializer;
    private readonly LiteDbMaskOptions _maskOptions;

    public LiteDbMaskManager(MaskCommunicationNode communicationNode, LiteDbMaskQueryManager queryManager, LiteDbMaskCommandManager commandManager, LiteDbMaskSchemaManager schemaManager, MaskPersistenceProvider persistenceProvider, LiteDbMaskOptions maskOptions, ILogger? logger = null) : base(communicationNode, queryManager, commandManager, schemaManager, persistenceProvider, maskOptions, logger)
    {
        _queryManager = queryManager;
        _schemaManager = schemaManager;
        _maskOptions = maskOptions;
        _logger = logger?.ResolveLogger<LiteDbMaskManager>();

        _databaseMaterializer = new DatabaseMaterializer();
    }

    public Result MaterializeDatabase(string? connectionString = null)
        => Results.AsResult(() =>
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

            return _databaseMaterializer.MaterializeDatabase(liteDb, _schemaManager.CurrentMaskedSchema.Value, _schemaManager.CurrentOutputSchema.Value, _queryManager);
        });
}
