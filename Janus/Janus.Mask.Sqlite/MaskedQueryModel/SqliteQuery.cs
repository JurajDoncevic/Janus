using Janus.Base;
using Janus.Commons.SchemaModels;
using Janus.Mask.MaskedQueryModel;

namespace Janus.Mask.Sqlite.MaskedQueryModel;

public sealed class SqliteQuery : MaskedQuery<TableauId, Unit, Unit, Unit>
{
    public SqliteQuery(TableauId startingWith, Unit selection, Unit joining, Unit projection) : base(startingWith, selection, joining, projection)
    {
    }
}