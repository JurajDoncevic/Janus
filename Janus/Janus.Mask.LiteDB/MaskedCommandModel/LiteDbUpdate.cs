using Janus.Base;
using Janus.Commons.SchemaModels;
using Janus.Mask.MaskedCommandModel;

namespace Janus.Mask.LiteDB.MaskedCommandModel;
public sealed class LiteDbUpdate : MaskedUpdate<Unit, Unit>
{
    private LiteDbUpdate(TableauId target, Unit selection, Unit mutation) : base(target, selection, mutation)
    {
    }
}
