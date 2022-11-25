using Janus.Wrapper.Sqlite.LocalCommanding;
using Janus.Wrapper.Sqlite.Translation;

namespace Janus.Wrapper.Sqlite;
public sealed class SqliteWrapperCommandManager : WrapperCommandManager<SqliteDelete, SqliteInsert, SqliteUpdate, string, string, string>
{
    public SqliteWrapperCommandManager(SqliteCommandTranslator commandTranslator, SqliteCommandExecutor commandExecutor)
        : base(commandTranslator, commandExecutor)
    {
    }
}
