using Janus.Base.Resulting;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Mask.LiteDB.Lenses;
using Janus.Mask.LiteDB.MaskedSchemaModel;
using LiteDB;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.Mask.LiteDB.Materialization;
public class DatabaseMaterializer
{
    public async Task<Result> MaterializeDatabase(LiteDatabase database, Database materializedSchema, DataSource currentSchema, LiteDbMaskQueryManager queryManager)
        => await Results.AsResult(async () =>
        {
            var tableauMappings =
                currentSchema.Schemas
                .SelectMany(s => s.Tableaus)
                .Select(t => (collectionName: $"{t.Schema.Name}_{t.Name}", tableauId: t.Id))
                .ToDictionary(t => t.collectionName, t => t.tableauId);

            foreach (var collection in materializedSchema.Collections)
            {
                var targetTableauId = tableauMappings[collection.Name];

                var dataAcquisitionQuery =
                    QueryModelBuilder.InitQueryOnDataSource(targetTableauId, currentSchema)
                    .WithSelection(conf => conf.WithExpression(TRUE()))
                    .Build();


                var queryResult = await queryManager.RunQuery(dataAcquisitionQuery);

                if (!queryResult)
                {
                    return Results.OnFailure($"Materialization failed to acquire data for tableau {dataAcquisitionQuery.OnTableauId}: {queryResult.Message}");
                }

                TabularDataLiteDbDataLens lens = TabularDataLiteDbDataLenses.Construct($"{targetTableauId}.");
                database.GetCollection(collection.Name).InsertBulk(
                   lens.Get(queryResult.Data).Data
                );
            }

            return Results.OnSuccess("Materialization completed");
        });
}
