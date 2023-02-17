namespace Janus.Mask.LocalQuerying;
public abstract class LocalQuery<TStartingWith, TSelection, TJoining, TProjection>
{
    private readonly TSelection _selection;
    private readonly TJoining _joining;
    private readonly TProjection _projection;
    private readonly TStartingWith _startingWith;

    protected LocalQuery(TStartingWith startingWith, TSelection selection, TJoining joining, TProjection projection)
    {
        _startingWith = startingWith;
        _selection = selection;
        _joining = joining;
        _projection = projection;
    }

    public TSelection Selection => _selection;

    public TJoining Joining => _joining;

    public TProjection Projection => _projection;

    public TStartingWith StartingWith => _startingWith;
}
