using Janus.Communication.Remotes;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Mediator.Persistence.LiteDB.DbModels;
internal sealed class RemotePointInfo
{
    [BsonId(false)]
    public string NodeId { get; init; }
    public string Address { get; init; }
    public int ListenPort { get; init; }
    public RemotePointTypes RemotePointType { get; init; }

    public override bool Equals(object? obj)
    {
        return obj is RemotePointInfo point &&
               NodeId == point.NodeId &&
               Address == point.Address &&
               ListenPort == point.ListenPort &&
               RemotePointType == point.RemotePointType;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(NodeId, Address, ListenPort, RemotePointType);
    }
}
