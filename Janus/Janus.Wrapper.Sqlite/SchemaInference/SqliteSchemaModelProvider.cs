using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.DataModels;
using Janus.Commons.SchemaModels;
using Janus.Wrapper.SchemaInference;
using Janus.Wrapper.SchemaInference.Model;
using Microsoft.Data.Sqlite;
using System.Data;

namespace Janus.Wrapper.Sqlite.SchemaInference;
public sealed class SqliteSchemaModelProvider : ISchemaModelProvider
{
    private readonly string _connectionString;

    public SqliteSchemaModelProvider(string connectionString)
    {
        _connectionString = connectionString;
    }

    public Result AttributeExists(string schemaName, string tableauName, string attributeName)
        => GetAttributes(schemaName, tableauName)
            .Bind(attributes => attributes.Where(a => a.Name.Equals(attributeName)).Count() > 0
                                    ? Results.OnSuccess($"Attribute {attributeName} on tableau {tableauName} exists")
                                    : Results.OnFailure($"Attribute {attributeName} on tableau {tableauName} doesn't exist"));

    public Result<IEnumerable<AttributeInfo>> GetAttributes(string schemaName, string tableauName)
        => Results.AsResult(() =>
        {
            if (!TableauExists(schemaName, tableauName))
                return Results.OnFailure<IEnumerable<AttributeInfo>>($"Tableau {tableauName} doesn't exist");

            var attributeInfos = Enumerable.Empty<AttributeInfo>();

            using var connection = new SqliteConnection(_connectionString);
            if (connection.State == System.Data.ConnectionState.Closed)
                connection.Open();
            using var command = connection.CreateCommand();

            // FROM doesn't take params, so a concat string has to be used
            // injection is avoided by checking if the tableau exists
            command.CommandText =
                $"SELECT * FROM pragma_table_info('{tableauName}');";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                attributeInfos =
                    attributeInfos.Append(
                        new AttributeInfo(
                            reader.GetFieldValue<string>(1), // name
                            InferDataType(reader.GetFieldValue<string>(2)), // type
                            reader.GetFieldValue<bool>(5), // pk
                            !reader.GetFieldValue<bool>(3),// nullable, notnull in db
                            reader.GetFieldValue<int>(0))// ordinal (cid)
                        );
            }

            using var checkTypesOnDataCommand = connection.CreateCommand();
            checkTypesOnDataCommand.CommandText =
                $"SELECT * FROM {tableauName} LIMIT 1;";

            using var checkTypeReader = checkTypesOnDataCommand.ExecuteReader();
            if (checkTypeReader.Read())
            {
                attributeInfos =
                attributeInfos.Fold(
                    Enumerable.Empty<AttributeInfo>(),
                    (attrInfo, attrInfos) =>
                    {
                        var type = checkTypeReader.GetFieldType(attrInfo.Name);
                        var adjustedDataType = TypeMappings.MapToDataType(type);
                        // if the inferred data type from the schema has a lower order than the adjusted data type
                        if (GetDataTypeInferenceOrder(attrInfo.DataType) < GetDataTypeInferenceOrder(adjustedDataType))
                        {
                            var adjustedAttributeInfo =
                                new AttributeInfo(
                                    attrInfo.Name,
                                    adjustedDataType,
                                    attrInfo.IsPrimaryKey,
                                    attrInfo.IsNullable,
                                    attrInfo.Ordinal
                                    );

                            return attrInfos.Append(adjustedAttributeInfo);
                        }
                        else
                        {
                            return attrInfos.Append(attrInfo);
                        }
                    });
            }


            return Results.OnSuccess(attributeInfos);
        });

    public Result<DataSourceInfo> GetDataSource()
    {
        return Results.OnFailure<DataSourceInfo>("No data source schemata in Sqlite, use the NodeId");
    }

    public Result<IEnumerable<SchemaInfo>> GetSchemas()
        => Results.AsResult(() =>
        {
            var schemaInfos = Enumerable.Empty<SchemaInfo>();

            using var connection = new SqliteConnection(_connectionString);
            if (connection.State == System.Data.ConnectionState.Closed)
                connection.Open();
            using var command = connection.CreateCommand();

            command.CommandText = "PRAGMA database_list;";

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                schemaInfos =
                    schemaInfos.Append(
                        new SchemaInfo(reader.GetFieldValue<string>(1))); // get `name`
            }

            return schemaInfos;
        });

    public Result<IEnumerable<TableauInfo>> GetTableaus(string schemaName)
        => Results.AsResult(() =>
        {
            var tableauInfos = Enumerable.Empty<TableauInfo>();

            using var connection = new SqliteConnection(_connectionString);
            if (connection.State == System.Data.ConnectionState.Closed)
                connection.Open();
            using var command = connection.CreateCommand();

            command.CommandText =
                "SELECT name " +
                "FROM sqlite_schema " +
                "WHERE type='table' AND name NOT LIKE 'sqlite%';"; // ignore some system tables

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                tableauInfos =
                    tableauInfos.Append(
                        new TableauInfo(reader.GetFieldValue<string>(0))); // get `name`
            }

            return tableauInfos;
        });

    public Result SchemaExists(string schemaName)
        => GetSchemas()
            .Bind(schemas => schemas.Where(s => s.Name.Equals(schemaName)).Count() > 0
                                ? Results.OnSuccess($"Schema {schemaName} exists")
                                : Results.OnFailure($"Schema {schemaName} doesn't exist"));

    public Result TableauExists(string schemaName, string tableauName) // ignores the schema name
        => Results.AsResult(() =>
        {
            using var connection = new SqliteConnection(_connectionString);
            if (connection.State == System.Data.ConnectionState.Closed)
                connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText =
                "SELECT EXISTS(" +
                "SELECT 1 " +
                "FROM sqlite_schema " +
                "WHERE type = 'table' AND name NOT LIKE 'sqlite%' AND name = $tableau_name);";

            command.Parameters.AddWithValue("$tableau_name", tableauName);

            using var reader = command.ExecuteReader();

            return reader.Read()
                ? reader.GetFieldValue<bool>(0)
                    ? Results.OnSuccess($"Tableau {tableauName} exists")
                    : Results.OnFailure($"Tableau {tableauName} doesn't exist")
                : Results.OnFailure("Failed to get result");
        });

    private DataTypes InferDataType(string sqliteDataTypeName)
        => sqliteDataTypeName.ToLower() switch
        {
            string name when name.Contains("int") => DataTypes.INT,
            string name when name.Contains("char") || name.Contains("text") || name.Contains("clob") => DataTypes.STRING,
            string name when name.Contains("blob") => DataTypes.BINARY,
            string name when name.Contains("real") || name.Contains("double") || name.Contains("float") || name.Contains("decimal") || name.Contains("numeric") => DataTypes.DECIMAL,
            string name when name.Contains("boolean") => DataTypes.BOOLEAN,
            string name when name.Contains("date") => DataTypes.DATETIME, // for DATE and DATETIME
            _ => DataTypes.STRING // defaults to string
        };

    /// <summary>
    /// Determines the order in which types are inferred. Inferred DataTypes can be upgraded to a higher order, but not downgraded
    /// </summary>
    /// <param name="dataType"></param>
    /// <returns></returns>
    private int GetDataTypeInferenceOrder(DataTypes dataType)
        => dataType switch
        {
            DataTypes.INT => 1,
            DataTypes.LONGINT => 2,
            DataTypes.DECIMAL => 3,
            DataTypes.STRING => 4,
            DataTypes.DATETIME => 5,
            DataTypes.BOOLEAN => 6,
            DataTypes.BINARY => 7,
            _ => 0
        };

}
