using Janus.Commons.SchemaModels;

namespace Janus.Mask.LocalCommanding;
public abstract class LocalInsert<TInstantiation> : LocalCommand
{
    private readonly TInstantiation _instantiation;

    protected LocalInsert(TInstantiation instantiation, TableauId target) : base(target)
    {
        _instantiation = instantiation;
    }

    public TInstantiation Instantiation => _instantiation;
}
