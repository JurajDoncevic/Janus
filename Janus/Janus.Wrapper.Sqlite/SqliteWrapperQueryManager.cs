using Janus.Wrapper.Sqlite.LocalDataModel;
using Janus.Wrapper.Sqlite.LocalQuerying;
using Janus.Wrapper.Sqlite.Translation;

namespace Janus.Wrapper.Sqlite;
public class SqliteWrapperQueryManager : WrapperQueryManager<string, string, string, SqliteTabularData, SqliteQuery>
{
    public SqliteWrapperQueryManager(SqliteQueryTranslator queryTranslator, SqliteDataTranslator dataTranslator, SqliteQueryExecutor queryExecutor) : base(queryTranslator, dataTranslator, queryExecutor)
    {
    }
}
