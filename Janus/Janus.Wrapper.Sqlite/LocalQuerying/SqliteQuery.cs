using Janus.Wrapper.LocalQuerying;

namespace Janus.Wrapper.Sqlite.LocalQuerying;
public class SqliteQuery : LocalQuery<string, string, string>
{
    internal SqliteQuery(string startingWithTable, string selection, string joining, string projection) : base(startingWithTable, selection, joining, projection)
    {
    }

    public string ToText()
        => $"{Projection.Trim()} {Joining.Trim()} {Selection.Trim()};";

}
