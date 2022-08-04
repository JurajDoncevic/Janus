using Janus.Wrapper.LocalQuerying;

namespace Janus.Wrapper.Sqlite.LocalQuerying;
internal class SqliteQuery : LocalQuery<string, string, string>
{
    internal SqliteQuery(string startingWithTable, string selection, string joining, string projection) : base(startingWithTable, selection, joining, projection)
    {
    }

    internal string ToText()
        => $"{Projection}\n{Joining}\n{Selection}\n;";

}
