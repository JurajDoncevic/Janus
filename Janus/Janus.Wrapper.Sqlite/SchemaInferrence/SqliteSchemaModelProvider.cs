using FunctionalExtensions.Base.Results;
using Janus.Commons.SchemaModels;
using Janus.Wrapper.SchemaInferrence;
using Janus.Wrapper.SchemaInferrence.Model;
using Microsoft.Data.Sqlite;

namespace Janus.Wrapper.Sqlite.SchemaInferrence;
public class SqliteSchemaModelProvider : ISchemaModelProvider
{
    private readonly string _connectionString;

    public SqliteSchemaModelProvider(string connectionString)
    {
        _connectionString = connectionString;
    }

    public Result AttributeExists(string schemaName, string tableauName, string attributeName)
        => GetAttributes(schemaName, tableauName)
            .Bind(attributes => attributes.Where(a => a.Name.Equals(attributeName)).Count() > 0
                                    ? Result.OnSuccess($"Attribute {attributeName} on tableau {tableauName} exists")
                                    : Result.OnFailure($"Attribute {attributeName} on tableau {tableauName} doesn't exist"));

    public Result<IEnumerable<AttributeInfo>> GetAttributes(string schemaName, string tableauName)
        => ResultExtensions.AsResult(() =>
        {
            if (!TableauExists(schemaName, tableauName))
                return Result<IEnumerable<AttributeInfo>>.OnFailure($"Tableau {tableauName} doesn't exist");

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

            return Result<IEnumerable<AttributeInfo>>.OnSuccess(attributeInfos);
        });

    public Result<DataSourceInfo> GetDataSource()
    {
        return Result<DataSourceInfo>.OnFailure("No data source schemata in Sqlite, use the NodeId");
    }

    public Result<IEnumerable<SchemaInfo>> GetSchemas()
        => ResultExtensions.AsResult(() =>
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
        => ResultExtensions.AsResult(() =>
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
                                ? Result.OnSuccess($"Schema {schemaName} exists")
                                : Result.OnFailure($"Schema {schemaName} doesn't exist"));

    public Result TableauExists(string schemaName, string tableauName) // ignores the schema name
        => ResultExtensions.AsResult(() =>
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
                    ? Result.OnSuccess($"Tableau {tableauName} exists")
                    : Result.OnFailure($"Tableau {tableauName} doesn't exist")
                : Result.OnFailure("Failed to get result");
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

}
