using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.CommandModels;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Communication.Remotes;
using Janus.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Mask;
public class MaskManager : IComponentManager
{
    public Option<DataSource> GetCurrentSchema()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<RemotePoint> GetRegisteredRemotePoints()
    {
        throw new NotImplementedException();
    }

    public Task<Result<RemotePoint>> RegisterRemotePoint(string address, int port)
    {
        throw new NotImplementedException();
    }

    public Task<Result> RunCommand(BaseCommand command)
    {
        throw new NotImplementedException();
    }

    public Task<Result<TabularData>> RunQuery(Query query)
    {
        throw new NotImplementedException();
    }

    public Task<Result<RemotePoint>> SendHello(RemotePoint remotePoint)
    {
        throw new NotImplementedException();
    }

    public Task<Result<RemotePoint>> SendHello(string address, int port)
    {
        throw new NotImplementedException();
    }

    public Task<Result> UnregisterRemotePoint(RemotePoint remotePoint)
    {
        throw new NotImplementedException();
    }
}
