using Janus.Base;
using Janus.Commons.SchemaModels;
using Janus.Mask.MaskedCommandModel;

namespace Janus.Mask.Sqlite.MaskedCommandModel;
public sealed class SqliteDelete : MaskedDelete<Unit>
{
    public SqliteDelete(TableauId target, Unit selection) : base(target, selection)
    {
    }
}
