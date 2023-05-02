﻿using Janus.Base.Resulting;
using Janus.Components.Persistence;
using Janus.Wrapper.Persistence.Models;

namespace Janus.Wrapper.Persistence;
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
