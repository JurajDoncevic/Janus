using Janus.Base.Resulting;
using Janus.Components.Persistence;
using Janus.Mask.Persistence.Models;

namespace Janus.Mask.Persistence;
/// <summary>
/// Interface for data source information persistence
/// </summary>
public interface IDataSourceInfoPersistence : IPersistence<string, DataSourceInfo>
{
    /// <summary>
    /// Gets the latest data source information from persistence
    /// </summary>
    /// <returns></returns>
    Result<DataSourceInfo> GetLatest();
}
