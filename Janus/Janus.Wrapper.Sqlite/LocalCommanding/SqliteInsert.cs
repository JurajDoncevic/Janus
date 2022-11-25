using Janus.Wrapper.LocalCommanding;

namespace Janus.Wrapper.Sqlite.LocalCommanding;
public sealed class SqliteInsert : LocalInsert<string>
{
    public SqliteInsert(string instantiation, string target) : base(instantiation, target)
    {
    }

    public string ToText()
        => $"INSERT INTO {Target} {Instantiation};";
}
