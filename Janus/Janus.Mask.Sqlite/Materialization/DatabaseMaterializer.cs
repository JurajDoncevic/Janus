using Janus.Base.Resulting;
using Janus.Commons.QueryModels;
using static Janus.Commons.SelectionExpressions.Expressions;
using Janus.Commons.SchemaModels;
using Janus.Mask.Sqlite.MaskedSchemaModel;
using Janus.Mask.Sqlite.Translation;
using Janus.Commons.CommandModels;
using Microsoft.Data.Sqlite;
using Janus.Base;
using Janus.Mask.Sqlite.MaskedDataModel;

namespace Janus.Mask.Sqlite.Materialization;
public sealed class DatabaseMaterializer
{
    public async Task<Result> MaterializeDatabase(string sqliteConnectionString, Database materializedSchema, DataSource currentSchema, SqliteMaskQueryManager queryManager)
        => await Results.AsResult(async () =>
        {
            var tableauMappings =
                currentSchema.Schemas
                .SelectMany(s => s.Tableaus)
                .Select(t => (collectionName: $"{t.Schema.Name}_{t.Name}", tableauId: t.Id))
                .ToDictionary(t => t.collectionName, t => t.tableauId);

            foreach (var table in materializedSchema.Tables)
            {
                var targetTableauId = tableauMappings[table.Name];

                var dataAcquisitionQuery =
                    QueryModelBuilder.InitQueryOnDataSource(targetTableauId, currentSchema)
                    .WithSelection(conf => conf.WithExpression(TRUE()))
                    .Build();


                var queryResult = await queryManager.RunQuery(dataAcquisitionQuery);

                if (!queryResult)
                {
                    return Results.OnFailure($"Materialization failed to acquire data for tableau {dataAcquisitionQuery.OnTableauId}: {queryResult.Message}");
                }

                var dataTranslator = new SqliteDataTranslator();
                var translation = dataTranslator.Translate(queryResult.Data);

                if (!translation)
                {
                    return Results.OnFailure($"Failed data translation for table {table.Name} with: {translation.Message}");
                }

                using var connection = new SqliteConnection(sqliteConnectionString);

                if (connection.State == System.Data.ConnectionState.Closed)
                    await connection.OpenAsync();

                // create table
                using var createTableCommand = connection.CreateCommand();
                createTableCommand.CommandText = GenerateCreateTableCommandText(table);
                var tableCreation = Results.AsResult(() => createTableCommand.ExecuteNonQuery() > -1);

                if (!tableCreation)
                {
                    return Results.OnFailure($"Materialization failed on table creation: {tableCreation.Message}");
                }

                // populate table
                using var insertIntoCommand = connection.CreateCommand();
                insertIntoCommand.CommandText = GenerateInsertCommandText(table, translation.Data);
                var tablePopulation = Results.AsResult(() => insertIntoCommand.ExecuteNonQuery() == translation.Data.ItemCount);

                if (!tablePopulation)
                {
                    return Results.OnFailure($"Materialization failed on table population: {tablePopulation.Message}");
                }
            }

            var relationshipMaterialization = Results.OnSuccess();
            // create relationships with alter table
            foreach (var relationship in materializedSchema.Relationships)
            {
                using var connection = new SqliteConnection(sqliteConnectionString);

                if (connection.State == System.Data.ConnectionState.Closed)
                    await connection.OpenAsync();

                // create relationship
                using var createRelationshipCommand = connection.CreateCommand();
                createRelationshipCommand.CommandText = GenerateAlterTableForRelationshipText(relationship);
                var relationshipCreation = Results.AsResult(() => createRelationshipCommand.ExecuteNonQuery() > -1);

                relationshipMaterialization = relationshipMaterialization.Bind(_ => relationshipCreation);
            }
            if (!relationshipMaterialization)
            {
                return Results.OnFailure($"Materialization failed on relationship creation: {relationshipMaterialization.Message}");
            }

            return Results.OnSuccess("Materialization completed");
        });


    private string GenerateAlterTableForRelationshipText(Relationship relationship)
    {
        string alterTableText = 
            $@"ALTER TABLE {relationship.ForeignKeyTableName} 
               ADD FOREIGN KEY ({relationship.ForeignKeyColumnName}) 
               REFERENCES {relationship.PrimaryKeyTableName}({relationship.PrimaryKeyColumnName});";

        return alterTableText;
    }

    private string GenerateInsertCommandText(Table table, SqliteTabularData data)
    {
        var dataAffs = data.DataSchema;
        string valueTuples = data.Data.Map(row =>
        {
            IEnumerable<string> valueReps =
                row.DataRow.Map(kv => ValueToStringRepresentation(kv.Value, dataAffs[kv.Key]));

            string valueTuple = $"({valueReps.Aggregate((s1, s2) => $"{s1},{s2}")})\n";

            return valueTuple;
        }).Aggregate((s1,s2) => $"{s1},{s2}");

        string commandText = $"INSERT INTO {table} VALUES {valueTuples};";

        return commandText;
    }

    private string GenerateCreateTableCommandText(Table table)
    {
        string columnDefinitionsText =
            table.Columns.OrderBy(c => c.Ordinal)
                .Map(c => $"{c.Name} {c.TypeAffinity} {(c.IsPrimaryKey ? "PRIMARY KEY" : string.Empty)}~n")
                .Aggregate((s1, s2) => $"{s1},{s2}");

        string commandText = $"CREATE TABLE {table.Name} (\n{columnDefinitionsText});";

        return commandText;
    }

    private string ValueToStringRepresentation(object? value, TypeAffinities expectedAffinity)
        => value is null 
            ? "NULL"
            : expectedAffinity switch
            {
                TypeAffinities.DATETIME => ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss"),
                TypeAffinities.TEXT => $"'{value}'",
                TypeAffinities.BLOB => $"X'{Convert.ToBase64String((byte[])value).ToLower()}'",
                TypeAffinities.BOOLEAN => ((bool)value) ? "1" : "0",
                _ => $"{value}"
            };
}
