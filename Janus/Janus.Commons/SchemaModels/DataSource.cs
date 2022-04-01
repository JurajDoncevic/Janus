﻿
namespace Janus.Commons.SchemaModels;

/// <summary>
/// Describes a data source
/// </summary>
public sealed class DataSource
{
    private readonly string _name;
    private readonly Dictionary<string, Schema> _schemas;

    /// <summary>
    /// Data source ID
    /// </summary>
    public string Id => _name;
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
    /// Get schema with name
    /// </summary>
    /// <param name="schemaName"></param>
    /// <returns></returns>
    public Schema this[string schemaName] => _schemas[schemaName];

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name">Data source name</param>
    internal DataSource(string name!!)
    {
        _name = name;
        _schemas = new();
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name">Data source name</param>
    /// <param name="schemas">Underlying schemas</param>
    internal DataSource(string name!!, List<Schema> schemas!!)
    {
        _name = name;
        _schemas = schemas.ToDictionary(schema => schema.Name, schema => schema);
    }

    /// <summary>
    /// Adds a new unique schema to the data source
    /// </summary>
    /// <param name="schema">Schema object</param>
    /// <returns>true if new schema is added, false if a schema with the given name exists</returns>
    internal bool AddSchema(Schema schema!!)
    {
        if (!_schemas.ContainsKey(schema.Name))
        {
            schema = new Schema(schema.Name, schema.Tableaus.ToList(), this);
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
    internal bool RemoveSchema(string schemaName!!)
    {
        return _schemas.Remove(schemaName);
    }

    public override bool Equals(object? obj)
    {
        return obj is DataSource source &&
               _name == source._name &&
               EqualityComparer<Dictionary<string, Schema>>.Default.Equals(_schemas, source._schemas);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_name, _schemas);
    }

    public static bool operator ==(DataSource? left, DataSource? right)
    {
        return EqualityComparer<DataSource>.Default.Equals(left, right);
    }

    public static bool operator !=(DataSource? left, DataSource? right)
    {
        return !(left == right);
    }
}
