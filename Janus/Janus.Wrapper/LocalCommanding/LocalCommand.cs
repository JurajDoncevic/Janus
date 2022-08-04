namespace Janus.Wrapper.LocalCommanding;
public abstract class LocalCommand
{
    private readonly string _target;

    protected LocalCommand(string target)
    {
        _target = target;
    }

    public string Target => _target;
}
