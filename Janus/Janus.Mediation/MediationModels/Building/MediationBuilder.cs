using Janus.Commons.SchemaModels;

namespace Janus.Mediation.MediationModels.Building;

/// <summary>
/// Mediation builder entry point
/// </summary>
public static class MediationBuilder
{
    /// <summary>
    /// Initiate mediation build for a declared data source
    /// </summary>
    /// <param name="dataSourceName">Declared mediated data source name</param>
    /// <param name="availableDataSources">Available data sources for this mediation</param>
    /// <returns>Data source builder instance</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public static IDataSourceMediationBuilder ForDataSource(string dataSourceName, IEnumerable<DataSource> availableDataSources)
    {
        if (string.IsNullOrWhiteSpace(dataSourceName))
        {
            throw new ArgumentException($"'{nameof(dataSourceName)}' cannot be null or whitespace.", nameof(dataSourceName));
        }

        if (availableDataSources is null)
        {
            throw new ArgumentNullException(nameof(availableDataSources));
        }

        return new DataSourceMediationBuilder(dataSourceName, availableDataSources.ToList());
    }
}
