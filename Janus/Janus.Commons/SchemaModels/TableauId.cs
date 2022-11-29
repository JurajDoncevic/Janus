using Janus.Commons.SchemaModels.Exceptions;

namespace Janus.Commons.SchemaModels;
/// <summary>
/// Tableau identifier made from schema models' names
/// </summary>
public sealed class TableauId
{
    private readonly string _tableauName;
    private readonly string _schemaName;
    private readonly string _dataSourceName;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="tableauName">Tableau name</param>
    /// <param name="schemaName">Schema name</param>
    /// <param name="dataSourceName">Data source name</param>
    /// <exception cref="ArgumentException"></exception>
    private TableauId(string dataSourceName, string schemaName, string tableauName)
    {
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

        _tableauName = tableauName;
        _schemaName = schemaName;
        _dataSourceName = dataSourceName;
    }

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
    public (string dataSourceName, string schemaName, string tableauName) NameTuple => (DataSourceName, SchemaName, TableauName);

    /// <summary>
    /// Parent schema identifier
    /// </summary>
    public SchemaId ParentSchemaId => SchemaId.From(_dataSourceName, _schemaName);
    /// <summary>
    /// Parent data source identifier
    /// </summary>
    public DataSourceId ParentDataSourceId => DataSourceId.From(_dataSourceName);

    /// <summary>
    /// Determines if the tableau identifier indicates it is a child of a schema with given id
    /// </summary>
    /// <param name="schemaId">Expected parent schema id</param>
    /// <returns></returns>
    public bool IsChildOf(SchemaId schemaId)
    {
        return _schemaName.Equals(schemaId.SchemaName) && 
               _dataSourceName.Equals(schemaId.DataSourceName);
    }

    /// <summary>
    /// Determines if the tableau identifier indicates it is a child of a data source with given id
    /// </summary>
    /// <param name="dataSourceId">Expected parent data source id</param>
    /// <returns></returns>
    public bool IsChildOf(DataSourceId dataSourceId)
    {
        return _dataSourceName.Equals(dataSourceId.DataSourceName);
    }

    /// <summary>
    /// Determines if the tableau identifier indicates it is a parent of an attribute with given id
    /// </summary>
    /// <param name="attributeId">Expected child attribute id</param>
    /// <returns></returns>
    public bool IsParentOf(AttributeId attributeId)
    {
        return _dataSourceName.Equals(attributeId.DataSourceName) &&
               _schemaName.Equals(attributeId.SchemaName) &&
               _tableauName.Equals(attributeId.TableauName);
    }

    public override bool Equals(object? obj)
    {
        return obj is TableauId id &&
               _tableauName == id._tableauName &&
               _schemaName == id._schemaName &&
               _dataSourceName == id._dataSourceName;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_tableauName, _schemaName, _dataSourceName);
    }

    /// <summary>
    /// String representation of the identifier (dot delimited)
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{_dataSourceName}.{_schemaName}.{_tableauName}";
    }

    /// <summary>
    /// Creates a tableau identifier from the given element names
    /// </summary>
    /// <param name="dataSourceName">Data source name</param>
    /// <param name="schemaName">Schema name</param>
    /// <param name="tableauName">Tableau name</param>
    /// <returns>Tableau identifier object</returns>
    public static TableauId From(string dataSourceName, string schemaName, string tableauName)
        => new TableauId(dataSourceName, schemaName, tableauName);

    /// <summary>
    /// Creates a tableau identifier from a parent schema id and tableau name
    /// </summary>
    /// <param name="parentSchemaId"></param>
    /// <param name="tableauName"></param>
    /// <returns></returns>
    public static TableauId From(SchemaId parentSchemaId, string tableauName)
        => new TableauId(parentSchemaId.DataSourceName, parentSchemaId.SchemaName, tableauName);

    /// <summary>
    /// Creates a tableau identifier from a dot-delimited string representation
    /// </summary>
    /// <param name="stringRepresentation">Dot-delimited string representation</param>
    /// <returns>Tableau identifier object</returns>
    /// <exception cref="InvalidIdStringException"></exception>
    public static TableauId From(string stringRepresentation)
        => stringRepresentation.Count(c => c == '.') == 2
            ? stringRepresentation.Split('.').Identity()
                .Map(stringSplit => new TableauId(stringSplit[0], stringSplit[1], stringSplit[2]))
                .Data
            : throw new InvalidIdStringException(stringRepresentation);
}
