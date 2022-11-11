using Janus.Commons.SchemaModels;

namespace Janus.Mediation.SchemaMediationModels.Building;

/// <summary>
/// Data source mediation builder interface
/// </summary>
public interface IDataSourceMediationBuilder
{
    /// <summary>
    /// Declares a schema mediation within the data source mediation
    /// </summary>
    /// <param name="schemaName">Declared mediated schema name</param>
    /// <param name="configuration">Schema mediation configuration via <see cref="ISchemaMediationBuilder"/></param>
    /// <returns>Current data source mediation builder</returns>
    IDataSourceMediationBuilder WithSchema(string schemaName, Func<ISchemaMediationBuilder, ISchemaMediationBuilder> configuration);
    /// <summary>
    /// Sets the declared name of the mediated data source
    /// </summary>
    /// <param name="dataSourceName">Declared mediated data source name</param>
    /// <returns>Current data source mediation builder</returns>
    IDataSourceMediationBuilder WithName(string dataSourceName);
    /// <summary>
    /// Sets the declared version of the mediated data source
    /// </summary>
    /// <param name="dataSourceVersion">Declared mediated data source version</param>
    /// <returns>Current data source mediation builder</returns>
    IDataSourceMediationBuilder WithVersion(string dataSourceVersion);
    /// <summary>
    /// Sets the declared description of the mediated data source
    /// </summary>
    /// <param name="dataSourceDescription">Declared mediated data source description</param>
    /// <returns>Current data source mediation builder</returns>
    IDataSourceMediationBuilder WithDescription(string? dataSourceDescription);
    /// <summary>
    /// Sets if the source update sets are to be propagated to the mediated tableaus
    /// </summary>
    /// <param name="propagateUpdateSets">Is propagation used?</param>
    /// <returns>Current data source mediation builder</returns>
    IDataSourceMediationBuilder WithUpdateSetPropagation(bool propagateUpdateSets);
    /// <summary>
    /// Sets if the source attribute descriptions are propagated to the mediated attributes
    /// </summary>
    /// <param name="propagateAttributeDescriptions"></param>
    /// <returns>Current data source mediation builder</returns>
    IDataSourceMediationBuilder WithAttributeDescriptionPropagation(bool propagateAttributeDescriptions);
    /// <summary>
    /// Builds the setup data source mediation
    /// </summary>
    /// <returns>Data source mediation object</returns>
    DataSourceMediation Build();
}

/// <summary>
/// Default data source mediation builder implementation
/// </summary>
internal class DataSourceMediationBuilder : IDataSourceMediationBuilder
{
    private string _dataSourceName;
    private string _dataSourceDescription;
    private string _dataSourceVersion;
    private bool _propagateUpdateSets = true;
    private bool _propagateAttributeDescriptions = false;
    private readonly List<DataSource> _availableDataSources;
    private readonly List<SchemaMediation> _schemaMediations;

    public DataSourceMediationBuilder(string dataSourceName,
                                      List<DataSource> availableDataSources,
                                      string? dataSourceDescription = null,
                                      string? dataSourceVersion = null)
    {
        if (string.IsNullOrEmpty(dataSourceName))
        {
            throw new ArgumentException($"'{nameof(dataSourceName)}' cannot be null or empty.", nameof(dataSourceName));
        }

        _dataSourceName = dataSourceName;
        _dataSourceDescription = dataSourceDescription ?? string.Empty;
        _dataSourceVersion = dataSourceVersion ?? Guid.NewGuid().ToString();
        _availableDataSources = availableDataSources ?? new();
        _schemaMediations = new();
    }

    public DataSourceMediation Build()
    {
        return new DataSourceMediation(_dataSourceName,
                                       _dataSourceDescription,
                                       _dataSourceVersion,
                                       _propagateUpdateSets,
                                       _propagateAttributeDescriptions,
                                       _availableDataSources,
                                       _schemaMediations);
    }

    public IDataSourceMediationBuilder WithDescription(string? dataSourceDescription)
    {
        _dataSourceDescription = dataSourceDescription ?? _dataSourceDescription;

        return this;
    }

    public IDataSourceMediationBuilder WithName(string dataSourceName)
    {
        _dataSourceName = dataSourceName ?? _dataSourceName;

        return this;
    }

    public IDataSourceMediationBuilder WithSchema(string declaredSchemaName, Func<ISchemaMediationBuilder, ISchemaMediationBuilder> configuration)
    {
        if (string.IsNullOrWhiteSpace(declaredSchemaName))
        {
            throw new ArgumentException($"'{nameof(declaredSchemaName)}' cannot be null or whitespace.", nameof(declaredSchemaName));
        }

        var schemaMediation = configuration(new SchemaMediationBuilder(declaredSchemaName)).Build();

        _schemaMediations.Add(schemaMediation);

        return this;
    }

    public IDataSourceMediationBuilder WithVersion(string dataSourceVersion)
    {
        _dataSourceVersion = dataSourceVersion ?? _dataSourceVersion;

        return this;
    }

    public IDataSourceMediationBuilder WithUpdateSetPropagation(bool propagateUpdateSets)
    {
        _propagateUpdateSets = propagateUpdateSets;

        return this;
    }

    public IDataSourceMediationBuilder WithAttributeDescriptionPropagation(bool propagateAttributeDescriptions)
    {
        _propagateAttributeDescriptions = propagateAttributeDescriptions;

        return this;
    }
}
