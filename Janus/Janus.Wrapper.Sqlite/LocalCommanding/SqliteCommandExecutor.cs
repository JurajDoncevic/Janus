using FunctionalExtensions.Base.Results;
using Janus.Wrapper.LocalCommanding;
using Microsoft.Data.Sqlite;

namespace Janus.Wrapper.Sqlite.LocalCommanding;
public class SqliteCommandExecutor : ICommandExecutor<SqliteDelete, SqliteInsert, SqliteUpdate, string, string, string>
{
    private readonly string _sqliteConnectionString;

    public SqliteCommandExecutor(string sqliteConnectionString)
    {
        _sqliteConnectionString = sqliteConnectionString;
    }

    public string SqliteConnectionString => _sqliteConnectionString;
    public async Task<Result> ExecuteDeleteCommand(SqliteDelete deleteCommand)
        => await ResultExtensions.AsResult(async () =>
        {
            using var connection = new SqliteConnection(_sqliteConnectionString);

            if (connection.State == System.Data.ConnectionState.Closed)
                await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = deleteCommand.ToText();

            return Result.OnSuccess($"Deleted {command.ExecuteNonQuery()} rows");
        });

    public async Task<Result> ExecuteInsertCommand(SqliteInsert insertCommand)
        => await ResultExtensions.AsResult(async () =>
        {
            using var connection = new SqliteConnection(_sqliteConnectionString);

            if (connection.State == System.Data.ConnectionState.Closed)
                await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = insertCommand.ToText();

            return Result.OnSuccess($"Inserted {command.ExecuteNonQuery()} rows");
        });

    public async Task<Result> ExecuteUpdateCommand(SqliteUpdate updateCommand)
        => await ResultExtensions.AsResult(async () =>
        {
            using var connection = new SqliteConnection(_sqliteConnectionString);

            if (connection.State == System.Data.ConnectionState.Closed)
                await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = updateCommand.ToText();

            return Result.OnSuccess($"Updated {command.ExecuteNonQuery()} rows");
        });
}
