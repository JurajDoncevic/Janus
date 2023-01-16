using Janus.Components.Persistence;

namespace Janus.Mask.Persistence;
public sealed class MaskPersistenceProvider
{
    private readonly IDataSourceInfoPersistence _dataSourceInfoPersistence;
    private readonly IRemotePointPersistence _remotePointPersistence;

    public MaskPersistenceProvider(IDataSourceInfoPersistence dataSourceInfoPersistence, IRemotePointPersistence remotePointPersistence)
    {
        _dataSourceInfoPersistence = dataSourceInfoPersistence;
        _remotePointPersistence = remotePointPersistence;
    }

    public IDataSourceInfoPersistence DataSourceInfoPersistence => _dataSourceInfoPersistence;

    public IRemotePointPersistence RemotePointPersistence => _remotePointPersistence;

}
