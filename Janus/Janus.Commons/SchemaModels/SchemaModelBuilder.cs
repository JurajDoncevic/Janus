using Janus.Commons.SchemaModels.Exceptions;

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
    public static DataSourceBuilder InitDataSource(string dataSourceName)
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

/// <summary>
/// Builder for a data source
/// </summary>
public class DataSourceBuilder
{
    private DataSource _dataSource;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dataSourceName">name of the initialized data source</param>
    internal DataSourceBuilder(string dataSourceName!!)
    {
        _dataSource = new DataSource(dataSourceName);
    }

    /// <summary>
    /// Adds a schema with give name and configuration to the data source
    /// </summary>
    /// <param name="schemaName">Schema name</param>
    /// <param name="configuration">Build configuration for the schema</param>
    /// <returns></returns>
    /// <exception cref="SchemaNameAssignedException"></exception>
    public DataSourceBuilder AddSchema(string schemaName, Func<SchemaBuilder, SchemaBuilder> configuration)
    {
        var schema = configuration(new SchemaBuilder(schemaName, _dataSource)).Build();

        if (_dataSource.SchemaNames.Contains(schemaName))
            throw new SchemaNameAssignedException(schemaName, _dataSource.Name);

        _dataSource.AddSchema(schema);
        return this;
    }

    /// <summary>
    /// Builds the data source
    /// </summary>
    /// <returns></returns>
    public DataSource Build()
    {
        return _dataSource;
    }
}

/// <summary>
/// Builder for a schema
/// </summary>
public class SchemaBuilder
{
    private Schema _schema;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="schemaName">Schema name</param>
    /// <param name="parentDataSource">Data source inside which the builder works</param>
    internal SchemaBuilder(string schemaName!!, DataSource parentDataSource!!)
    {
        _schema = new Schema(schemaName, parentDataSource);
    }

    /// <summary>
    /// Adds a tableau with given name and configuration to the schema
    /// </summary>
    /// <param name="tableauName">tableau name</param>
    /// <param name="configuration">Build configuration</param>
    /// <returns></returns>
    /// <exception cref="TableauNameAssignedException"></exception>
    public SchemaBuilder AddTableau(string tableauName, Func<TableauBuilder, TableauBuilder> configuration)
    {
        var tableau = configuration(new TableauBuilder(tableauName, _schema)).Build();

        if (_schema.TableauNames.Contains(tableauName))
            throw new TableauNameAssignedException(tableauName, _schema.Name);

        _schema.AddTableau(tableau);
        return this;
    }
    /// <summary>
    /// Builds the schema
    /// </summary>
    /// <returns></returns>
    internal Schema Build()
    {
        return _schema;
    }
}

/// <summary>
/// Builder for a tableau
/// </summary>
public class TableauBuilder
{
    private Tableau _tableau;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="tableauName">Tableau name</param>
    /// <param name="parentSchema">Schema inside which the builder works</param>
    internal TableauBuilder(string tableauName, Schema parentSchema)
    {
        _tableau = new Tableau(tableauName, parentSchema);
    }

    /// <summary>
    /// Adds an attribute with given name and configuration
    /// </summary>
    /// <param name="attributeName">Attribute name</param>
    /// <param name="configuration">Build configuration</param>
    /// <returns></returns>
    /// <exception cref="AttributeOrdinalAssignedException"></exception>
    /// <exception cref="AttributeNameAssignedException"></exception>
    public TableauBuilder AddAttribute(string attributeName, Func<AttributeBuilder, AttributeBuilder> configuration)
    {
        var attribute = configuration(new AttributeBuilder(attributeName, _tableau)).Build();

        if(_tableau.Attributes.ToList().Exists(a => a.Ordinal == attribute.Ordinal))
            throw new AttributeOrdinalAssignedException(attribute.Ordinal, attributeName, _tableau.Name);
        if (_tableau.AttributeNames.Contains(attributeName))
            throw new AttributeNameAssignedException(attributeName, _tableau.Name);

        _tableau.AddAttribute(attribute);
        return this;
    }

    /// <summary>
    /// Builds the tableau
    /// </summary>
    /// <returns></returns>
    internal Tableau Build()
    {
        return _tableau;
    }
}

/// <summary>
/// Builder for an attribute
/// </summary>
public class AttributeBuilder
{
    private string _attributeName;
    private DataTypes _attributeDataType;
    private int _attributeOrdinal = -1;
    private bool _isAttributePrimaryKey = false;
    private bool _isAttributeNullable = true;
    private Tableau _parentTableau;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="attributeName">Attribute</param>
    /// <param name="parentTableau">Tableau inside which the builder works</param>
    internal AttributeBuilder(string attributeName!!, Tableau parentTableau!!)
    {
        _attributeName = attributeName;
        _parentTableau = parentTableau;
    }

    /// <summary>
    /// Sets the attribute's name
    /// </summary>
    /// <param name="name">Attribute name</param>
    /// <returns></returns>
    public AttributeBuilder WithName(string name!!)
    {
        _attributeName = name;
        return this;
    }

    /// <summary>
    /// Sets the attribute's data type
    /// </summary>
    /// <param name="dataType">Data type</param>
    /// <returns></returns>
    public AttributeBuilder WithDataType(DataTypes dataType)
    {
        _attributeDataType = dataType;
        return this;
    }

    /// <summary>
    /// Sets the attribute's ordinal number
    /// </summary>
    /// <param name="ordinal">Ordinal number</param>
    /// <returns></returns>
    /// <exception cref="AttributeOrdinalOutOfRange"></exception>
    public AttributeBuilder WithOrdinal(int ordinal)
    {
        if (ordinal < 0)
            throw new AttributeOrdinalOutOfRange(_attributeName);

        _attributeOrdinal = ordinal;
        return this;
    }

    /// <summary>
    /// Sets whether the attribute is or part of a primary key
    /// </summary>
    /// <param name="isPrimaryKey"></param>
    /// <returns></returns>
    public AttributeBuilder WithIsPrimaryKey(bool isPrimaryKey)
    {
        _isAttributePrimaryKey = isPrimaryKey;
        return this;
    }

    /// <summary>
    /// Sets whether the attribute value is nullable
    /// </summary>
    /// <param name="isNullable"></param>
    /// <returns></returns>
    public AttributeBuilder WithIsNullable(bool isNullable)
    {
        _isAttributeNullable = isNullable;
        return this;
    }

    /// <summary>
    /// Builds the attribute
    /// </summary>
    /// <returns></returns>
    internal Attribute Build()
    {
        return new Attribute(_attributeName, _attributeDataType, _isAttributePrimaryKey, _isAttributeNullable, _attributeOrdinal, _parentTableau);
    }
}
