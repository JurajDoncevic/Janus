using FunctionalExtensions.Base.Results;
using Janus.Wrapper.LocalQuerying;
using Janus.Wrapper.Sqlite.LocalDataModel;
using Microsoft.Data.Sqlite;

namespace Janus.Wrapper.Sqlite.LocalQuerying;
public class SqliteQueryExecutor : IQueryExecutor<string, string, string, SqliteTabularData, SqliteQuery>
{
    private readonly string _sqliteConnectionString;

    public SqliteQueryExecutor(string sqliteConnectionString)
    {
        _sqliteConnectionString = sqliteConnectionString;
    }

    public string SqliteConnectionString => _sqliteConnectionString;

    public async Task<Result<SqliteTabularData>> ExecuteQuery(SqliteQuery localQuery)
        => await ResultExtensions.AsResult(async () =>
        {
            using var connection = new SqliteConnection(_sqliteConnectionString);

            if (connection.State == System.Data.ConnectionState.Closed)
                await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = localQuery.ToText();

            using var reader = await command.ExecuteReaderAsync();
            var results = new List<Dictionary<string, (Type, object)>>(); // {column name, (type, value)} 
            var rowSchema = new Dictionary<string, Type>();
            while (await reader.ReadAsync())
            {
                if (rowSchema.Count == 0)
                {
                    foreach (var col in await reader.GetColumnSchemaAsync())
                    {
                        rowSchema.Add($"{col.BaseCatalogName}.{col.BaseTableName}.{col.ColumnName}", col.DataType!);
                    }
                }

                var row = new Dictionary<string, (Type, object)>();
                foreach (var columnName in rowSchema.Keys)
                {
                    row.Add(columnName, (rowSchema[columnName], reader.GetValue(rowSchema.Keys.ToList().IndexOf(columnName))));
                }

                results.Add(row);
            }

            return new SqliteTabularData { Data = results };
        });
}
