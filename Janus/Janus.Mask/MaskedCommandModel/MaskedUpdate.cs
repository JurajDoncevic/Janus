using Janus.Commons.SchemaModels;

namespace Janus.Mask.MaskedCommandModel;
public abstract class MaskedUpdate<TSelection, TMutation> : MaskedCommand
{
    private readonly TSelection _selection;
    private readonly TMutation _mutation;

    protected MaskedUpdate(TableauId target, TSelection selection, TMutation mutation) : base(target)
    {
        _selection = selection;
        _mutation = mutation;
    }

    public TSelection Selection => _selection;

    public TMutation Mutation => _mutation;
}
