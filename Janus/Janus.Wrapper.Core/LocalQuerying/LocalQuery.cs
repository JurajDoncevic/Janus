namespace Janus.Wrapper.Core.LocalQuerying;
public abstract class LocalQuery<TSelection, TJoining, TProjection>
{
    private readonly TSelection _selection;
    private readonly TJoining _joining;
    private readonly TProjection _projection;

    protected LocalQuery(TSelection selection, TJoining joining, TProjection projection)
    {
        _selection = selection;
        _joining = joining;
        _projection = projection;
    }

    public TSelection Selection => _selection;

    public TJoining Joining => _joining;

    public TProjection Projection => _projection;
}
