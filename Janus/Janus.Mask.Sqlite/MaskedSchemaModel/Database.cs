using Janus.Mask.MaskedSchemaModel;

namespace Janus.Mask.Sqlite.MaskedSchemaModel;
public sealed class Database : MaskedDataSource
{
    private readonly string _name;
    private readonly Dictionary<string, Table> _tables;
    private readonly HashSet<Relationship> _relationships;

    internal Database(string name, IEnumerable<Table>? tables = null, IEnumerable<Relationship>? relationships = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
        }

        _name = name;
        _tables = tables?.ToDictionary(c => c.Name, c => c) ?? new Dictionary<string, Table>();
        _relationships = relationships?.ToHashSet() ?? new HashSet<Relationship>();
    }

    public string Name => _name;

    public IReadOnlyList<Table> Tables => _tables.Values.ToList();

    public IReadOnlySet<Relationship> Relationships => _relationships;

    public Table this[string tableName] => _tables[tableName];

    internal bool AddTable(Table table)
    {
        if (table is null)
        {
            throw new ArgumentNullException(nameof(table));
        }

        return _tables.TryAdd(table.Name, table);
    }

    internal bool RemoveTable(string tableName)
    {
        return _tables.Remove(tableName);
    }

    public bool AddRelationship(Relationship relationship)
    {
        if (relationship is null)
        {
            throw new ArgumentNullException(nameof(relationship));
        }

        if(this[relationship.PrimaryKeyTableName]?[relationship.PrimaryKeyColumnName] == null ||
           this[relationship.ForeignKeyTableName]?[relationship.ForeignKeyColumnName] == null)
        {
            return false;
        }

        if (_relationships.Contains(relationship))
        {
            return false;
        }

        _relationships.Add(relationship);

        return true;
    }

    public bool RemoveRelationship(Relationship relationship)
    {
        if (relationship is null)
        {
            throw new ArgumentNullException(nameof(relationship));
        }

        return _relationships.Add(relationship);
    }
}
