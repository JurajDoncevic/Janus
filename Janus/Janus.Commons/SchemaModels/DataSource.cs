namespace Janus.Commons.SchemaModels;

/// <summary>
/// Describes a data source
/// </summary>
public sealed class DataSource
{
    private readonly DataSourceId _id;
    private readonly string _name;
    private readonly Dictionary<string, Schema> _schemas;
    private readonly string _description;
    private readonly string _version;

    /// <summary>
    /// Data source ID
    /// </summary>
    public DataSourceId Id => _id;
    /// <summary>
    /// Data source name
    /// </summary>
    public string Name => _name;
    /// <summary>
    /// All schemas in the data source
    /// </summary>
    public ReadOnlyCollection<Schema> Schemas => _schemas.Values.ToList().AsReadOnly();
    /// <summary>
    /// Names of schemas in this data source
    /// </summary>
    public List<string> SchemaNames => _schemas.Keys.ToList();
    /// <summary>
    /// Data source version
    /// </summary>
    public string Version => _version;
    /// <summary>
    /// Data source description
    /// </summary>
    public string Description => _description;

    /// <summary>
    /// Get schema with name
    /// </summary>
    /// <param name="schemaName"></param>
    /// <returns></returns>
    public Schema this[string schemaName] => _schemas[schemaName];

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name">Data source name</param>
    internal DataSource(string name, string description, string version)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        }
        _id = DataSourceId.From(name);
        _name = name;
        _schemas = new();
        _description = description;
        _version = String.IsNullOrEmpty(version) ? Guid.NewGuid().ToString() : version;
    }


    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name">Data source name</param>
    /// <param name="schemas">Underlying schemas</param>
    internal DataSource(string name, string description, string version, List<Schema> schemas)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        }

        if (schemas is null)
        {
            throw new ArgumentNullException(nameof(schemas));
        }
        _id = DataSourceId.From(name);
        _name = name;
        _description = description ?? string.Empty;
        _version = String.IsNullOrEmpty(version) ? Guid.NewGuid().ToString() : version;
        _schemas = schemas.ToDictionary(schema => schema.Name, schema => schema);
    }

    /// <summary>
    /// Adds a new unique schema to the data source
    /// </summary>
    /// <param name="schema">Schema object</param>
    /// <returns>true if new schema is added, false if a schema with the given name exists</returns>
    internal bool AddSchema(Schema schema)
    {
        if (schema is null)
        {
            throw new ArgumentNullException(nameof(schema));
        }

        if (!_schemas.ContainsKey(schema.Name))
        {
            schema = new Schema(schema.Name, schema.Tableaus.ToList(), this, schema.Description);
            _schemas.Add(schema.Name, schema);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Removes a schema with the given name
    /// </summary>
    /// <param name="schemaName">Schema name</param>
    /// <returns>true if a schema is found and removed, else false</returns>
    internal bool RemoveSchema(string schemaName)
    {
        if (schemaName is null)
        {
            throw new ArgumentNullException(nameof(schemaName));
        }

        return _schemas.Remove(schemaName);
    }

    /// <summary>
    /// Checks if the data source contains a schema with given name  
    /// </summary>
    /// <param name="schemaName">Schema name</param>
    /// <returns></returns>
    public bool ContainsSchema(string schemaName)
        => _schemas.ContainsKey(schemaName);

    /// <summary>
    /// Checks if the data source contains a schema with given id  
    /// </summary>
    /// <param name="schemaId">Schema id</param>
    /// <returns></returns>
    public bool ContainsSchema(SchemaId schemaId)
        => _schemas.Values.Any(schema => schema.Id.Equals(schemaId));

    /// <summary>
    /// Checks if the data source contains a tableau with given ID  
    /// </summary>
    /// <param name="tableuId">Tableau ID</param>
    /// <returns></returns>
    public bool ContainsTableau(TableauId tableuId)
        => _schemas.Values.SelectMany(schema => schema.Tableaus)
                          .Select(tableau => tableau.Id)
                          .Contains(tableuId);

    /// <summary>
    /// Checks if the data source contains an attribute with given ID  
    /// </summary>
    /// <param name="attributeId">Attribute ID</param>
    /// <returns></returns>
    public bool ContainsAttribute(AttributeId attributeId)
        => _schemas.Values.SelectMany(schema => schema.Tableaus)
                          .SelectMany(tableau => tableau.Attributes)
                          .Select(attribute => attribute.Id)
                          .Contains(attributeId);

    public override bool Equals(object? obj)
    {
        return obj is DataSource source &&
               _id.Equals(source._id) &&
               _name.Equals(source._name) &&
               _version.Equals(source._version) &&
               _description.Equals(source._description) &&
               _schemas.SequenceEqual(source._schemas);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_id, _name, _schemas, _version, _description);
    }

    public static bool operator ==(DataSource? left, DataSource? right)
    {
        return EqualityComparer<DataSource>.Default.Equals(left, right);
    }

    public static bool operator !=(DataSource? left, DataSource? right)
    {
        return !(left == right);
    }

    public override string ToString()
        => $"({Name} \n({string.Join("\n", Schemas)}))";
}
