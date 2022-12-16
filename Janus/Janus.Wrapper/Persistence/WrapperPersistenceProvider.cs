using Janus.Components.Persistence;

namespace Janus.Wrapper.Persistence;
public class WrapperPersistenceProvider
{
    private readonly IDataSourceInfoPersistence _dataSourceInfoPersistence;
    private readonly IRemotePointPersistence _remotePointPersistence;

    public WrapperPersistenceProvider(IDataSourceInfoPersistence dataSourceInfoPersistence, IRemotePointPersistence remotePointPersistence)
    {
        _dataSourceInfoPersistence = dataSourceInfoPersistence;
        _remotePointPersistence = remotePointPersistence;
    }

    public IDataSourceInfoPersistence DataSourceInfoPersistence => _dataSourceInfoPersistence;

    public IRemotePointPersistence RemotePointPersistence => _remotePointPersistence;
}
