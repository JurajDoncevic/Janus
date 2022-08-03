namespace Janus.Wrapper.Core.LocalCommanding;
public abstract class LocalDelete<TSelection> : LocalCommand
{
    private readonly TSelection _selection;

    protected LocalDelete(string target, TSelection selection) : base(target)
    {
        _selection = selection;
    }

    public TSelection Selection => _selection;
}
