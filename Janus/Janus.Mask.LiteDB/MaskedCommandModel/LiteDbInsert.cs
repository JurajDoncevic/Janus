using FunctionalExtensions.Base;
using Janus.Commons.SchemaModels;
using Janus.Mask.MaskedCommandModel;

namespace Janus.Mask.LiteDB.MaskedCommandModel;
public class LiteDbInsert : MaskedInsert<Unit>
{
    private LiteDbInsert(Unit instantiation, TableauId target) : base(instantiation, target)
    {
    }
}
