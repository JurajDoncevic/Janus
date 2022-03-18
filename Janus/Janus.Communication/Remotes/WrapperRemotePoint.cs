using System.Net;

namespace Janus.Communication.Remotes;

public class WrapperRemotePoint : RemotePoint
{
    public WrapperRemotePoint(string id, string address, int port) : base(id, address, port)
    {
    }
}
