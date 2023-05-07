using Janus.Base;
using Janus.Commons.SchemaModels;
using Janus.Mask.MaskedCommandModel;

namespace Janus.Mask.Sqlite.MaskedCommandModel;
public sealed class SqliteInsert : MaskedInsert<Unit>
{
    public SqliteInsert(Unit instantiation, TableauId target) : base(instantiation, target)
    {
    }
}
