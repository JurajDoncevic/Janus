using Janus.Commons.SchemaModels.Exceptions;

namespace Janus.Commons.SchemaModels.Building;
/// <summary>
/// Builder for a data source
/// </summary>
public sealed class DataSourceBuilder : IDataSourceBuilder
{
    private DataSource? _dataSource;
    private string _dataSourceName;
    private string _dataSourceDescription;
    private string _dataSourceVersion;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dataSourceName">Name of the initialized data source</param>
    /// <param name="dataSourceDescription">Description of the data source</param>
    /// <param name="dataSourceVersion">Version of the data source</param>
    internal DataSourceBuilder(string dataSourceName, string? dataSourceDescription = "", string? dataSourceVersion = null)
    {
        if (string.IsNullOrEmpty(dataSourceName))
        {
            throw new ArgumentException($"'{nameof(dataSourceName)}' cannot be null or empty.", nameof(dataSourceName));
        }
        _dataSourceName = dataSourceName;
        _dataSourceDescription = dataSourceDescription ?? "";
        _dataSourceVersion = dataSourceVersion ?? Guid.NewGuid().ToString();

        _dataSource = null;
    }

    /// <summary>
    /// Adds a schema with give name and configuration to the data source
    /// </summary>
    /// <param name="schemaName">Schema name</param>
    /// <param name="configuration">Build configuration for the schema</param>
    /// <returns></returns>
    /// <exception cref="SchemaNameAssignedException"></exception>
    public ISchemaAdding AddSchema(string schemaName, Func<ISchemaBuilder, ISchemaBuilding> configuration)
    {
        // on first add initiate the data source
        if (_dataSource == null)
        {
            _dataSource = new DataSource(_dataSourceName, _dataSourceDescription, _dataSourceVersion);
        }

        var schema = configuration(new SchemaBuilder(schemaName, _dataSource)).Build();

        if (_dataSource.SchemaNames.Contains(schemaName))
            throw new SchemaNameAssignedException(schemaName, _dataSource.Name);

        _dataSource.AddSchema(schema);
        return this;
    }
    /// <summary>
    /// Sets the data source name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IDataSourceEditing WithName(string name)
    {
        _dataSourceName = name ?? _dataSourceName;

        return this;
    }

    /// <summary>
    /// Sets the data source description
    /// </summary>
    /// <param name="description"></param>
    /// <returns></returns>
    public IDataSourceEditing WithDescription(string description)
    {
        _dataSourceDescription = description ?? _dataSourceDescription;

        return this;
    }

    /// <summary>
    /// Sets the data source version
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    public IDataSourceEditing WithVersion(string version)
    {
        _dataSourceVersion = version ?? _dataSourceVersion;

        return this;
    }

    /// <summary>
    /// Builds the data source
    /// </summary>
    /// <returns>Built data source</returns>
    public DataSource Build()
    {
        return _dataSource ?? new DataSource(_dataSourceName, _dataSourceDescription, _dataSourceVersion);
    }
}

public interface IDataSourceBuilding
{
    /// <summary>
    /// Builds the data source
    /// </summary>
    /// <returns>Built data source</returns>
    public DataSource Build();
}

public interface IDataSourceEditing : ISchemaAdding
{
    /// <summary>
    /// Sets the data source name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IDataSourceEditing WithName(string name);
    /// <summary>
    /// Sets the data source description
    /// </summary>
    /// <param name="description"></param>
    /// <returns></returns>
    IDataSourceEditing WithDescription(string description);
    /// <summary>
    /// Sets the data source version
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    IDataSourceEditing WithVersion(string version);
}

public interface ISchemaAdding : IDataSourceBuilding
{
    /// <summary>
    /// Adds a schema with give name and configuration to the data source
    /// </summary>
    /// <param name="schemaName">Schema name</param>
    /// <param name="configuration">Build configuration for the schema</param>
    /// <returns></returns>
    /// <exception cref="SchemaNameAssignedException"></exception>
    ISchemaAdding AddSchema(string schemaName, Func<ISchemaBuilder, ISchemaBuilding> configuration);
}

public interface IDataSourceBuilder : IDataSourceEditing
{

}