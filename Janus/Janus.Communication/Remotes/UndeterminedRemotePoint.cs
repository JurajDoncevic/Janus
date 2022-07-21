namespace Janus.Communication.Remotes;
public class UndeterminedRemotePoint : RemotePoint
{
    public UndeterminedRemotePoint(string address, int listenPort) : base(address, listenPort)
    {
    }


    public override RemotePointTypes RemotePointType => RemotePointTypes.UNDETERMINED;
}
