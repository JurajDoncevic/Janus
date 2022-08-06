using Janus.Wrapper.LocalCommanding;

namespace Janus.Wrapper.Sqlite.LocalCommanding;
public class SqliteUpdate : LocalUpdate<string, string>
{
    public SqliteUpdate(string target, string selection, string mutation) : base(target, selection, mutation)
    {
    }
}
