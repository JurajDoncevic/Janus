namespace Janus.Commons.Communication.Remotes;

public abstract class RemotePoint
{
    private readonly string _id;
    private readonly string _address;

    protected RemotePoint(string id, string address)
    {
        _id = id;
        _address = address;
    }

    public string Id => _id;

    public string Address => _address;
}
