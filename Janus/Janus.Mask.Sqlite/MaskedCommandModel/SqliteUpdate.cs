using Janus.Base;
using Janus.Commons.SchemaModels;
using Janus.Mask.MaskedCommandModel;

namespace Janus.Mask.Sqlite.MaskedCommandModel;
public sealed class SqliteUpdate : MaskedUpdate<Unit, Unit>
{
    public SqliteUpdate(TableauId target, Unit selection, Unit mutation) : base(target, selection, mutation)
    {
    }
}
