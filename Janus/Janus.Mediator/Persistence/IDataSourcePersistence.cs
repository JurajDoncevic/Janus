using FunctionalExtensions.Base.Resulting;
using Janus.Commons.SchemaModels;
using Janus.Components.Persistence;
using Janus.Mediator.Persistence.Models;

namespace Janus.Mediator.Persistence;
public interface IDataSourcePersistence : IPersistence<string, DataSourceInfo>
{
    Result<DataSourceInfo> GetLatest();
}
