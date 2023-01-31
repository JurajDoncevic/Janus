using FunctionalExtensions.Base.Resulting;
using Janus.Wrapper.LocalQuerying;
using Janus.Wrapper.Sqlite.LocalDataModel;
using Microsoft.Data.Sqlite;
using System.Data.Common;

namespace Janus.Wrapper.Sqlite.LocalQuerying;
public sealed class SqliteQueryExecutor : IQueryExecutor<string, string, string, SqliteTabularData, SqliteQuery>
{
    private readonly string _sqliteConnectionString;

    public SqliteQueryExecutor(string sqliteConnectionString)
    {
        _sqliteConnectionString = sqliteConnectionString;
    }

    public string SqliteConnectionString => _sqliteConnectionString;

    public async Task<Result<SqliteTabularData>> ExecuteQuery(SqliteQuery localQuery)
        => await Results.AsResult(async () =>
        {
            using var connection = new SqliteConnection(_sqliteConnectionString);

            if (connection.State == System.Data.ConnectionState.Closed)
                await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = localQuery.ToText();

            using var reader = await command.ExecuteReaderAsync();
            var dataRows = new List<Dictionary<string, object?>>();
            var dataSchema = new Dictionary<string, Type>();

            while (await reader.ReadAsync())
            {
                if (dataSchema.Count == 0)
                {
                    foreach (var col in await reader.GetColumnSchemaAsync())
                    {
                        var columnType = GetColumnTypeByDataTypeName(col.DataTypeName!);

                        dataSchema.Add($"{col.BaseCatalogName}.{col.BaseTableName}.{col.ColumnName}", columnType);
                    }
                }

                var row = new Dictionary<string, object?>();
                foreach (var columnName in dataSchema.Keys)
                {
                    var columnIndex = dataSchema.Keys.ToList().IndexOf(columnName);
                    var fieldValue = ReadFieldWithType(reader, columnIndex, dataSchema[columnName]);
                    row.Add(columnName, fieldValue);
                }

                dataRows.Add(row);
            }

            return new SqliteTabularData(dataSchema, dataRows);
        });

    private object? ReadFieldWithType(SqliteDataReader reader, int columnIndex, Type expectedType)
        => reader.IsDBNull(columnIndex) 
            ? null 
            : expectedType switch
            {
                Type type when type.Equals(typeof(long)) => reader.GetInt64(columnIndex),
                Type type when type.Equals(typeof(double)) => reader.GetDouble(columnIndex),
                Type type when type.Equals(typeof(string)) => reader.GetString(columnIndex),
                Type type when type.Equals(typeof(byte[])) => (object)reader.GetFieldValue<byte[]>(columnIndex),
                Type type when type.Equals(typeof(DateTime)) => reader.GetDateTime(columnIndex),
                Type type when type.Equals(typeof(bool)) => reader.GetBoolean(columnIndex),
                _ => reader.GetDouble(columnIndex)
            };

    private Type GetColumnTypeByDataTypeName(string dataTypeName)
        => dataTypeName switch
        {
            string dtn when dtn.Contains("INT") => typeof(long),
            string dtn when dtn.Contains("REAL") || dtn.Contains("FLOA") || dtn.Contains("DOUB") => typeof(double),
            string dtn when dtn.Contains("CHAR") || dtn.Contains("CLOB") || dtn.Contains("TEXT") => typeof(string),
            string dtn when dtn.Contains("BLOB") => typeof(byte[]),
            string dtn when dtn.Contains("DATE") => typeof(DateTime),
            string dtn when dtn.Contains("BOOL") => typeof(bool),
            _ => typeof(double)
        };

}
