using System.Net;

namespace Janus.Communication.Remotes;

public class MaskRemotePoint : RemotePoint
{
    public MaskRemotePoint(string address, int port) : base(address, port)
    {
    }

    public MaskRemotePoint(string id, string address, int port) : base(id, address, port)
    {
    }
}
