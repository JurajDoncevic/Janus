using System.Net;

namespace Janus.Communication.Remotes;

public abstract class RemotePoint
{
    private readonly string _id;
    private readonly string _address;
    private readonly int _port;

    public string Id => _id;
    public string Address => _address;
    public int Port => _port;

    internal RemotePoint(string id, string address, int port)
    {
        _id = id;
        _address = address;
        _port = port;
    }

    public override bool Equals(object? obj)
    {
        return obj is RemotePoint point &&
               _id == point._id &&
               EqualityComparer<string>.Default.Equals(_address, point._address) &&
               _port == point._port &&
               Id == point.Id &&
               EqualityComparer<string>.Default.Equals(Address, point.Address) &&
               Port == point.Port;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_id, _address, _port, Id, Address, Port);
    }
}
