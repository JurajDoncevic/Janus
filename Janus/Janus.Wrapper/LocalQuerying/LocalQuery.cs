namespace Janus.Wrapper.LocalQuerying;
public abstract class LocalQuery<TSelection, TJoining, TProjection>
{
    private readonly TSelection _selection;
    private readonly TJoining _joining;
    private readonly TProjection _projection;
    private readonly string _startingWith;

    protected LocalQuery(string startingWith, TSelection selection, TJoining joining, TProjection projection)
    {
        _startingWith = startingWith;
        _selection = selection;
        _joining = joining;
        _projection = projection;
    }

    public TSelection Selection => _selection;

    public TJoining Joining => _joining;

    public TProjection Projection => _projection;

    public string StartingWith => _startingWith;
}
