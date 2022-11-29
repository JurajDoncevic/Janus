using Janus.Commons.SchemaModels.Exceptions;

namespace Janus.Commons.SchemaModels;

/// <summary>
/// Schema identifier made from schema models' names
/// </summary>
public sealed class SchemaId
{
    private readonly string _schemaName;
    private readonly string _dataSourceName;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="schemaName">Schema name</param>
    /// <param name="dataSourceName">Data source name</param>
    /// <exception cref="ArgumentException"></exception>
    private SchemaId(string dataSourceName, string schemaName)
    {
        if (string.IsNullOrWhiteSpace(schemaName) || schemaName.Contains('.'))
        {
            throw new ArgumentException($"'{nameof(schemaName)}' cannot be null, or whitespace, or contain dots..", nameof(schemaName));
        }

        if (string.IsNullOrWhiteSpace(dataSourceName) || dataSourceName.Contains('.'))
        {
            throw new ArgumentException($"'{nameof(dataSourceName)}' cannot be null, or whitespace, or contain dots.", nameof(dataSourceName));
        }

        _schemaName = schemaName;
        _dataSourceName = dataSourceName;
    }

    /// <summary>
    /// Schema name
    /// </summary>
    public string SchemaName => _schemaName;
    /// <summary>
    /// Data source name
    /// </summary>
    public string DataSourceName => _dataSourceName;

    /// <summary>
    /// Tuple of all schema model element names
    /// </summary>
    public (string dataSourceName, string schemaName) NameTuple => (DataSourceName, SchemaName);

    /// <summary>
    /// Parent data source identifier
    /// </summary>
    public DataSourceId ParentDataSourceId => DataSourceId.From(_dataSourceName);

    /// <summary>
    /// Determines if the schema identifier indicates it is a child of a data source with given id
    /// </summary>
    /// <param name="dataSourceId">Expected parent data source id</param>
    /// <returns></returns>
    public bool IsChildOf(DataSourceId dataSourceId)
    {
        return _dataSourceName.Equals(dataSourceId.DataSourceName);
    }

    /// <summary>
    /// Determines if the schema identifier indicates it is a parent of a tableau with given id
    /// </summary>
    /// <param name="tableauId">Expected child tableau id</param>
    /// <returns></returns>
    public bool IsParentOf(TableauId tableauId)
    {
        return _schemaName.Equals(tableauId.SchemaName) &&
               _dataSourceName.Equals(tableauId.DataSourceName);
    }
    /// <summary>
    /// Determines if the schema identifier indicates it is a parent of an attribute with given id
    /// </summary>
    /// <param name="attributeId">Expected child attribute id</param>
    /// <returns></returns>
    public bool IsParentOf(AttributeId attributeId)
    {
        return _schemaName.Equals(attributeId.SchemaName) &&
               _dataSourceName.Equals(attributeId.DataSourceName);
    }

    public override bool Equals(object? obj)
    {
        return obj is SchemaId id &&
               _schemaName == id._schemaName &&
               _dataSourceName == id._dataSourceName;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_schemaName, _dataSourceName);
    }

    /// <summary>
    /// String representation of the identifier (dot delimited)
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{_dataSourceName}.{_schemaName}";
    }

    /// <summary>
    /// Creates a schema identifier from given element names
    /// </summary>
    /// <param name="dataSourceName">Data source name</param>
    /// <param name="schemaName">Schema name</param>
    /// <returns>Schema identifier object</returns>
    public static SchemaId From(string dataSourceName, string schemaName)
        => new SchemaId(dataSourceName, schemaName);

    /// <summary>
    /// Creates a schema identifier from a given data source id and schema name
    /// </summary>
    /// <param name="parentDataSourceId"></param>
    /// <param name="schemaName"></param>
    /// <returns></returns>
    public static SchemaId From(DataSourceId parentDataSourceId, string schemaName)
        => new SchemaId(parentDataSourceId.DataSourceName, schemaName);

    /// <summary>
    /// Creates a schema identifier from given dot-delimited string representation
    /// </summary>
    /// <param name="stringRepresentation">Dot-delimited string representation</param>
    /// <returns>Schema identifier object</returns>
    /// <exception cref="InvalidIdStringException"></exception>
    public static SchemaId From(string stringRepresentation)
        => stringRepresentation.Count(c => c == '.') == 1
            ? stringRepresentation.Split('.').Identity()
                .Map(stringSplit => new SchemaId(stringSplit[0], stringSplit[1]))
                .Data
            : throw new InvalidIdStringException(stringRepresentation);
}
