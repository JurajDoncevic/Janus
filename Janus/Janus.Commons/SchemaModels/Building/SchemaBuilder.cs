using Janus.Commons.SchemaModels.Exceptions;

namespace Janus.Commons.SchemaModels.Building;

/// <summary>
/// Builder for a schema
/// </summary>
public sealed class SchemaBuilder : ISchemaBuilder
{
    private Schema? _schema;
    private string _schemaName;
    private string _schemaDescription;
    private DataSource _parentDataSource;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="schemaName">Schema name</param>
    /// <param name="parentDataSource">Data source inside which the builder works</param>
    internal SchemaBuilder(string schemaName, DataSource parentDataSource, string? schemaDescription = "")
    {
        if (string.IsNullOrEmpty(schemaName))
        {
            throw new ArgumentException($"'{nameof(schemaName)}' cannot be null or empty.", nameof(schemaName));
        }

        if (parentDataSource is null)
        {
            throw new ArgumentNullException(nameof(parentDataSource));
        }
        _schemaName = schemaName;
        _parentDataSource = parentDataSource;
        _schemaDescription = schemaDescription ?? string.Empty;
        _schema = null;
    }

    /// <summary>
    /// Adds a tableau with given name and configuration to the schema
    /// </summary>
    /// <param name="tableauName">tableau name</param>
    /// <param name="configuration">Build configuration</param>
    /// <returns></returns>
    /// <exception cref="TableauNameAssignedException"></exception>
    public ITableauAdding AddTableau(string tableauName, Func<ITableauBuilder, ITableauBuilding> configuration)
    {
        if (_schema is null)
        {
            _schema = new Schema(_schemaName, _parentDataSource, _schemaDescription);
        }

        var tableau = configuration(new TableauBuilder(tableauName, _schema)).Build();

        if (_schema.TableauNames.Contains(tableauName))
            throw new TableauNameAssignedException(tableauName, _schema.Name);

        _schema.AddTableau(tableau);
        return this;
    }
    /// <summary>
    /// Sets the schema description
    /// </summary>
    /// <param name="description"></param>
    /// <returns></returns>
    public ISchemaEditing WithDescription(string description)
    {
        _schemaDescription = description ?? _schemaDescription;
        return this;
    }
    /// <summary>
    /// Sets the schema name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public ISchemaEditing WithName(string name)
    {
        _schemaName = name ?? _schemaName;
        return this;
    }

    /// <summary>
    /// Builds the schema
    /// </summary>
    /// <returns>Built Schema</returns>
    public Schema Build()
    {
        return _schema ?? new Schema(_schemaName, _parentDataSource, _schemaDescription);
    }
}

public interface ISchemaBuilding
{
    /// <summary>
    /// Builds the schema
    /// </summary>
    /// <returns>Built Schema</returns>
    Schema Build();
}

public interface ISchemaEditing : ITableauAdding
{
    /// <summary>
    /// Sets the schema name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    ISchemaEditing WithName(string name);
    /// <summary>
    /// Sets the schema description
    /// </summary>
    /// <param name="description"></param>
    /// <returns></returns>
    ISchemaEditing WithDescription(string description);

}

public interface ITableauAdding : ISchemaBuilding
{
    /// <summary>
    /// Adds a tableau with given name and configuration to the schema
    /// </summary>
    /// <param name="tableauName">tableau name</param>
    /// <param name="configuration">Build configuration</param>
    /// <returns></returns>
    /// <exception cref="TableauNameAssignedException"></exception>
    ITableauAdding AddTableau(string tableauName, Func<ITableauBuilder, ITableauBuilding> configuration);
}


public interface ISchemaBuilder : ISchemaEditing
{

}
