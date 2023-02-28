using Janus.Commons.SchemaModels;

namespace Janus.Mask.MaskedCommandModel;
public abstract class MaskedInsert<TInstantiation> : MaskedCommand
{
    private readonly TInstantiation _instantiation;

    protected MaskedInsert(TInstantiation instantiation, TableauId target) : base(target)
    {
        _instantiation = instantiation;
    }

    public TInstantiation Instantiation => _instantiation;
}
