namespace Janus.Wrapper.Core.LocalCommanding;
public abstract class LocalUpdate<TSelection, TMutation> : LocalCommand
{
    private readonly TSelection _selection;
    private readonly TMutation _mutation;

    protected LocalUpdate(string target, TSelection selection, TMutation mutation) : base(target)
    {
        _selection = selection;
        _mutation = mutation;
    }

    public TSelection Selection => _selection;

    public TMutation Mutation => _mutation;
}
