using Janus.Commons.SchemaModels.Exceptions;

namespace Janus.Commons.SchemaModels;

/// <summary>
/// Attribute identifier made from schema models' names
/// </summary>
public class AttributeId
{
    private readonly string _attributeName;
    private readonly string _tableauName;
    private readonly string _schemaName;
    private readonly string _dataSourceName;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="attributeName">Attribute name</param>
    /// <param name="tableauName">Tableau name</param>
    /// <param name="schemaName">Schema name</param>
    /// <param name="dataSourceName">Data source name</param>
    /// <exception cref="ArgumentException"></exception>
    private AttributeId(string dataSourceName, string schemaName, string tableauName, string attributeName)
    {
        if (string.IsNullOrWhiteSpace(attributeName) || attributeName.Contains('.'))
        {
            throw new ArgumentException($"'{nameof(attributeName)}' cannot be null, or whitespace, or contain dots.", nameof(attributeName));
        }

        if (string.IsNullOrWhiteSpace(tableauName) || tableauName.Contains('.'))
        {
            throw new ArgumentException($"'{nameof(tableauName)}' cannot be null, or whitespace, or contain dots.", nameof(tableauName));
        }

        if (string.IsNullOrWhiteSpace(schemaName) || schemaName.Contains('.'))
        {
            throw new ArgumentException($"'{nameof(schemaName)}' cannot be null, or whitespace, or contain dots..", nameof(schemaName));
        }

        if (string.IsNullOrWhiteSpace(dataSourceName) || dataSourceName.Contains('.'))
        {
            throw new ArgumentException($"'{nameof(dataSourceName)}' cannot be null, or whitespace, or contain dots.", nameof(dataSourceName));
        }

        _attributeName = attributeName;
        _tableauName = tableauName;
        _schemaName = schemaName;
        _dataSourceName = dataSourceName;
    }

    /// <summary>
    /// Attribute name
    /// </summary>
    public string AttributeName => _attributeName;
    /// <summary>
    /// Tableau name
    /// </summary>
    public string TableauName => _tableauName;
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
    public (string dataSourceName, string schemaName, string tableauName, string attributeName) NameTuple => (DataSourceName, SchemaName, TableauName, AttributeName);

    /// <summary>
    /// Parent tableau identifier
    /// </summary>
    public TableauId ParentTableauId => TableauId.From(_dataSourceName, _schemaName, _tableauName);
    /// <summary>
    /// Parent schema identifier
    /// </summary>
    public SchemaId ParentSchemaId => SchemaId.From(_dataSourceName, _schemaName);
    /// <summary>
    /// Parent data source identifier
    /// </summary>
    public DataSourceId ParentDataSourceId => DataSourceId.From(_dataSourceName);

    /// <summary>
    /// Determines if the attribute identifier indicates it is a child of a tableau with given id
    /// </summary>
    /// <param name="tableauId">Expected parent tableau id</param>
    /// <returns></returns>
    public bool IsChildOf(TableauId tableauId)
    {
        return _tableauName.Equals(tableauId.TableauName) &&
               _schemaName.Equals(tableauId.SchemaName) &&
               _dataSourceName.Equals(tableauId.DataSourceName);
    }

    /// <summary>
    /// Determines if the attribute identifier indicates it is a child of a schema with given id
    /// </summary>
    /// <param name="schemaId">Expected parent schema id</param>
    /// <returns></returns>
    public bool IsChildOf(SchemaId schemaId)
    {
        return _schemaName.Equals(schemaId.SchemaName) &&
               _dataSourceName.Equals(schemaId.DataSourceName);
    }

    /// <summary>
    /// Determines if the attribute identifier indicates it is a child of a data source with given id
    /// </summary>
    /// <param name="dataSourceId">Expected parent data source id</param>
    /// <returns></returns>
    public bool IsChildOf(DataSourceId dataSourceId)
    {
        return _dataSourceName.Equals(dataSourceId.DataSourceName);
    }

    /// <summary>
    /// String representation of the identifier (dot delimited)
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{_dataSourceName}.{_schemaName}.{_tableauName}.{_attributeName}";
    }

    public override bool Equals(object? obj)
    {
        return obj is AttributeId id &&
               _attributeName == id._attributeName &&
               _tableauName == id._tableauName &&
               _schemaName == id._schemaName &&
               _dataSourceName == id._dataSourceName;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_attributeName, _tableauName, _schemaName, _dataSourceName);
    }

    /// <summary>
    /// Creates an attribute identifier from the given element names
    /// </summary>
    /// <param name="dataSourceName">Data source name</param>
    /// <param name="schemaName">Schema name</param>
    /// <param name="tableauName">Tableau name</param>
    /// <param name="attributeName">Attribute name</param>
    /// <returns>Attribute identifier object</returns>
    public static AttributeId From(string dataSourceName, string schemaName, string tableauName, string attributeName)
        => new AttributeId(dataSourceName, schemaName, tableauName, attributeName);

    /// <summary>
    /// Creates an attribute identifier from the given dot-delimited string representation
    /// </summary>
    /// <param name="stringRepresentation">Dot-delimited string identifier representation</param>
    /// <returns>Attribute identifier object</returns>
    /// <exception cref="InvalidIdStringException"></exception>
    public static AttributeId From(string stringRepresentation)
        => stringRepresentation.Count(c => c == '.') == 3
            ? stringRepresentation.Split('.').Identity()
                .Map(stringSplit => new AttributeId(stringSplit[0], stringSplit[1], stringSplit[2], stringSplit[3]))
                .Data
            : throw new InvalidIdStringException(stringRepresentation);
}
