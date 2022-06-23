using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Communication.Remotes;
public class UndeterminedRemotePoint : RemotePoint
{
    public UndeterminedRemotePoint(string address, int listenPort) : base(address, listenPort)
    {
    }


    public override RemotePointTypes RemotePointType => RemotePointTypes.UNDETERMINED;
}
