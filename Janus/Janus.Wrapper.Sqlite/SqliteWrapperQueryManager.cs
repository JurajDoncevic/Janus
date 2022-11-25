using Janus.Wrapper.Sqlite.LocalDataModel;
using Janus.Wrapper.Sqlite.LocalQuerying;
using Janus.Wrapper.Sqlite.Translation;

namespace Janus.Wrapper.Sqlite;
public sealed class SqliteWrapperQueryManager : WrapperQueryManager<SqliteQuery, string, string, string, SqliteTabularData>
{
    public SqliteWrapperQueryManager(SqliteQueryTranslator queryTranslator, SqliteDataTranslator dataTranslator, SqliteQueryExecutor queryExecutor) : base(queryTranslator, dataTranslator, queryExecutor)
    {
    }
}
