using Janus.Wrapper.LocalCommanding;

namespace Janus.Wrapper.Sqlite.LocalCommanding;
public sealed class SqliteDelete : LocalDelete<string>
{
    public SqliteDelete(string target, string selection) : base(target, selection)
    {
    }

    public string ToText()
        => $"DELETE FROM {Target} {Selection};";
}
