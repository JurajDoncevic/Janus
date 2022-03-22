﻿using System.Net;

namespace Janus.Communication.Remotes;

public class MediatorRemotePoint : RemotePoint
{
    public MediatorRemotePoint(string address, int port) : base(address, port)
    {
    }

    public MediatorRemotePoint(string id, string address, int port) : base(id, address, port)
    {
    }
}
