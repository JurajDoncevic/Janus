using Janus.Commons.SchemaModels;

namespace Janus.Mask.MaskedCommandModel;
public abstract class MaskedDelete<TSelection> : MaskedCommand
{
    private readonly TSelection _selection;

    protected MaskedDelete(TableauId target, TSelection selection) : base(target)
    {
        _selection = selection;
    }

    public TSelection Selection => _selection;
}
