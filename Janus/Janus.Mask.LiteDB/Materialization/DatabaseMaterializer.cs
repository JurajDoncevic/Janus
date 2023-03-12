using FunctionalExtensions.Base.Resulting;
using Janus.Commons.SchemaModels;
using Janus.Mask.LiteDB.MaskedSchemaModel;
using LiteDB;

namespace Janus.Mask.LiteDB.Materialization;
public class DatabaseMaterializer
{
    public Result MaterializeDatabase(LiteDatabase database, Database materializedSchema, DataSource currentSchema, LiteDbMaskQueryManager queryManager)
    {
        return Results.OnFailure();
    }
}
