using Janus.Components.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Mediator.Persistence;
public class MediatorPersistenceProvider
{
    private readonly IDataSourceInfoPersistence _dataSourceInfoPersistence;
    private readonly IRemotePointPersistence _remotePointPersistence;

    public MediatorPersistenceProvider(IDataSourceInfoPersistence dataSourceInfoPersistence, IRemotePointPersistence remotePointPersistence)
    {
        _dataSourceInfoPersistence = dataSourceInfoPersistence;
        _remotePointPersistence = remotePointPersistence;
    }

    public IDataSourceInfoPersistence DataSourceInfoPersistence => _dataSourceInfoPersistence;

    public IRemotePointPersistence RemotePointPersistence => _remotePointPersistence;
}
