using Janus.Commons.SchemaModels;

namespace Janus.Mask.LocalCommanding;
public abstract class LocalDelete<TSelection> : LocalCommand
{
    private readonly TSelection _selection;

    protected LocalDelete(TableauId target, TSelection selection) : base(target)
    {
        _selection = selection;
    }

    public TSelection Selection => _selection;
}
