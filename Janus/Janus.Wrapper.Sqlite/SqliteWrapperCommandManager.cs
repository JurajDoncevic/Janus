using Janus.Wrapper.LocalCommanding;
using Janus.Wrapper.Sqlite.LocalCommanding;
using Janus.Wrapper.Sqlite.Translation;
using Janus.Wrapper.Translation;

namespace Janus.Wrapper.Sqlite;
public class SqliteWrapperCommandManager : WrapperCommandManager<SqliteDelete, SqliteInsert, SqliteUpdate, string, string, string>
{
    public SqliteWrapperCommandManager(SqliteCommandTranslator commandTranslator, SqliteCommandExecutor commandExecutor) 
        : base(commandTranslator, commandExecutor)
    {
    }
}
