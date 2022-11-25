using Janus.Wrapper.LocalCommanding;

namespace Janus.Wrapper.Sqlite.LocalCommanding;
public sealed class SqliteUpdate : LocalUpdate<string, string>
{
    public SqliteUpdate(string target, string selection, string mutation) : base(target, selection, mutation)
    {
    }

    public string ToText()
        => $"UPDATE {Target} {Mutation} {Selection};";
}
