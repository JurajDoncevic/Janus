using Janus.Commons.SchemaModels.Building;

namespace Janus.Commons.SchemaModels;

/// <summary>
/// Builder for a schema model
/// </summary>
public class SchemaModelBuilder
{
    private DataSourceBuilder _dataSourceBuilder;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dataSourceBuilder">Data source builder with a initialized data source</param>
    private SchemaModelBuilder(DataSourceBuilder dataSourceBuilder)
    {
        _dataSourceBuilder = dataSourceBuilder;
    }
    /// <summary>
    /// Initializes a DataSourceBuilder with a data source
    /// </summary>
    /// <param name="dataSourceName">Data source name</param>
    /// <returns></returns>
    public static IDataSourceBuilder InitDataSource(string dataSourceName)
    {
        var dataSourceBuilder = new DataSourceBuilder(dataSourceName);

        return new SchemaModelBuilder(dataSourceBuilder)._dataSourceBuilder;
    }

    /// <summary>
    /// Build the final data source
    /// </summary>
    /// <returns></returns>
    public DataSource Build()
    {
        return _dataSourceBuilder.Build();
    }
}
