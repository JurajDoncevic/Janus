namespace Janus.Commons.SchemaModels;

/// <summary>
/// data source identifier made from schema models' names
/// </summary>
public sealed class DataSourceId
{
    private readonly string _dataSourceName;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dataSourceName">Data source name</param>
    private DataSourceId(string dataSourceName)
    {
        if (string.IsNullOrWhiteSpace(dataSourceName) || dataSourceName.Contains('.'))
        {
            throw new ArgumentException($"'{nameof(dataSourceName)}' cannot be null, or whitespace, or contain dots.", nameof(dataSourceName));
        }

        _dataSourceName = dataSourceName;
    }

    /// <summary>
    /// Data source name
    /// </summary>
    public string DataSourceName => _dataSourceName;

    /// <summary>
    /// Determines if the data source identifier indicates it is a parent of a tableau with given id
    /// </summary>
    /// <param name="tableauId">Expected child tableau id</param>
    /// <returns></returns>
    public bool IsParentOf(TableauId tableauId)
    {
        return _dataSourceName.Equals(tableauId.DataSourceName);
    }
    /// <summary>
    /// Determines if the data source identifier indicates it is a parent of a schema with given id
    /// </summary>
    /// <param name="schemaId">Expected child schema id</param>
    /// <returns></returns>
    public bool IsParentOf(SchemaId schemaId)
    {
        return _dataSourceName.Equals(schemaId.DataSourceName);
    }
    /// <summary>
    /// Determines if the data source identifier indicates it is a parent of an attribute with given id
    /// </summary>
    /// <param name="attributeId">Expected child attribute id</param>
    /// <returns></returns>
    public bool IsParentOf(AttributeId attributeId)
    {
        return _dataSourceName.Equals(attributeId.DataSourceName);
    }

    public override bool Equals(object? obj)
    {
        return obj is DataSourceId id &&
               _dataSourceName == id._dataSourceName;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_dataSourceName);
    }
    /// <summary>
    /// String representation of the identifier (dot delimited)
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return _dataSourceName;
    }
    /// <summary>
    /// Creates a data source identifier from the given name
    /// </summary>
    /// <param name="dataSourceName">Data source name</param>
    /// <returns>Data source identifier object</returns>
    public static DataSourceId From(string dataSourceName)
        => new DataSourceId(dataSourceName);
}
