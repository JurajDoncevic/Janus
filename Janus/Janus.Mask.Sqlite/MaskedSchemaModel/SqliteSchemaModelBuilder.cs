using Janus.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Mask.Sqlite.MaskedSchemaModel;
public static class SqliteSchemaModelBuilder
{
    public static DatabaseBuilder Init(string databaseName)
    {
        return new DatabaseBuilder(databaseName);
    }
}

public sealed class DatabaseBuilder
{
    private string _databaseName;
    private Dictionary<string, Table> _databaseTables;
    private HashSet<Relationship> _databaseRelationships;

    internal DatabaseBuilder(string databaseName, IEnumerable<Table>? databaseTables = null, IEnumerable<Relationship>? databaseRelationships = null)
    {
        _databaseName = databaseName;
        _databaseTables = databaseTables?.ToDictionary(c => c.Name, c => c) ?? new Dictionary<string, Table>();
        _databaseRelationships = databaseRelationships?.ToHashSet() ?? new HashSet<Relationship>();
    }

    public DatabaseBuilder WithName(string databaseName)
    {
        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new ArgumentException($"'{nameof(databaseName)}' cannot be null or whitespace.", nameof(databaseName));
        }

        _databaseName = databaseName;

        return this;
    }

    public DatabaseBuilder AddTable(string tableName, Func<TableBuilder, TableBuilder> configuration)
    {
        if (string.IsNullOrWhiteSpace(tableName))
        {
            throw new ArgumentException($"'{nameof(tableName)}' cannot be null or whitespace.", nameof(tableName));
        }

        var builder = new TableBuilder(tableName);

        var table = configuration(builder).Build();

        _databaseTables.Add(table.Name, table);

        return this;

    }

    public DatabaseBuilder AddRelationship(Func<RelationshipBuilder, RelationshipBuilder> configuration)
    {
        var builder = new RelationshipBuilder(_databaseTables);
        var relationship = configuration(builder).Build();

        _databaseRelationships.Add(relationship);

        return this;
    }

    public Database Build()
    {
        return new Database(_databaseName, _databaseTables.Values, _databaseRelationships);
    }
}

public sealed class RelationshipBuilder
{
    private Option<string> _foreignKeyColumnName;
    private Option<string> _foreignKeyTableName;
    private Option<string> _primaryKeyColumnName;
    private Option<string> _primaryKeyTableName;

    private readonly IReadOnlyDictionary<string, Table> _databaseTables;

    public RelationshipBuilder(Dictionary<string, Table> databaseTables)
    {
        _databaseTables = databaseTables ?? new Dictionary<string, Table>();
        _foreignKeyColumnName = Option<string>.None;
        _foreignKeyTableName = Option<string>.None;
        _primaryKeyColumnName = Option<string>.None;
        _primaryKeyTableName = Option<string>.None;
    }

    public RelationshipBuilder WithForeignKey(string foreignKeyTableName, string foreignKeyColumnName)
    {
        if (string.IsNullOrWhiteSpace(foreignKeyTableName))
        {
            throw new ArgumentException($"'{nameof(foreignKeyTableName)}' cannot be null or whitespace.", nameof(foreignKeyTableName));
        }

        if (string.IsNullOrWhiteSpace(foreignKeyColumnName))
        {
            throw new ArgumentException($"'{nameof(foreignKeyColumnName)}' cannot be null or whitespace.", nameof(foreignKeyColumnName));
        }
        _foreignKeyColumnName = Option<string>.Some(foreignKeyColumnName);
        _foreignKeyTableName = Option<string>.Some(foreignKeyTableName);

        return this;
    }

    public RelationshipBuilder WithPrimaryKey(string primaryKeyTableName, string primaryKeyColumnName)
    {
        if (string.IsNullOrWhiteSpace(primaryKeyTableName))
        {
            throw new ArgumentException($"'{nameof(primaryKeyTableName)}' cannot be null or whitespace.", nameof(primaryKeyTableName));
        }

        if (string.IsNullOrWhiteSpace(primaryKeyColumnName))
        {
            throw new ArgumentException($"'{nameof(primaryKeyColumnName)}' cannot be null or whitespace.", nameof(primaryKeyColumnName));
        }
        _primaryKeyColumnName = Option<string>.Some(primaryKeyColumnName);
        _primaryKeyTableName = Option<string>.Some(primaryKeyTableName);

        return this;
    }

    internal Relationship Build()
    {
        if(!_primaryKeyColumnName || !_primaryKeyTableName || !_foreignKeyColumnName || !_foreignKeyTableName)
        {
            throw new Exception("Some elements of the relationship have not been declared");
        }
        return new Relationship(_foreignKeyTableName.Value, _foreignKeyColumnName.Value, _primaryKeyTableName.Value, _primaryKeyColumnName.Value);
    }
}

public sealed class TableBuilder
{
    private string _tableName;
    private Dictionary<string, Column> _tableColumns;

    internal TableBuilder(string tableName, IEnumerable<Column>? columns = null)
    {
        _tableName = tableName;
        _tableColumns = columns?.ToDictionary(_ => _.Name, _ => _) ?? new Dictionary<string, Column>();
    }

    public TableBuilder WithName(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName))
        {
            throw new ArgumentException($"'{nameof(tableName)}' cannot be null or whitespace.", nameof(tableName));
        }
        _tableName = tableName;

        return this;
    }

    public TableBuilder AddColumn(string columnName, Func<ColumnBuilder, ColumnBuilder> configuration)
    {
        if (string.IsNullOrWhiteSpace(columnName))
        {
            throw new ArgumentException($"'{nameof(columnName)}' cannot be null or whitespace.", nameof(columnName));
        }

        var builder = new ColumnBuilder(columnName, _tableColumns.Any() ? _tableColumns.Max(c => c.Value.Ordinal) + 1 : 0);

        var column = configuration(builder).Build();

        _tableColumns.Add(column.Name, column);

        return this;
    }

    internal Table Build()
    {
        return new Table(_tableName, _tableColumns.Values);
    }
}

public sealed class ColumnBuilder
{
    private string _columnName;
    private bool _columnIsPrimaryKey = false;
    private int _columnOrdinal = 0;
    private TypeAffinities _typeAffinity = TypeAffinities.TEXT;

    internal ColumnBuilder(string name, int ordinal)
    {
        _columnName = name;
        _columnOrdinal = ordinal;
    }

    public ColumnBuilder AsPrimaryKey()
    {
        _columnIsPrimaryKey = true;

        return this;
    }

    public ColumnBuilder WithName(string columnName)
    {
        if (string.IsNullOrWhiteSpace(columnName))
        {
            throw new ArgumentException($"'{nameof(columnName)}' cannot be null or whitespace.", nameof(columnName));
        }

        _columnName = columnName;

        return this;
    }

    public ColumnBuilder WithOrdinal(int ordinal)
    {
        if (ordinal < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ordinal));
        }

        _columnOrdinal = ordinal;

        return this;
    }

    public ColumnBuilder WithTypeAffinity(TypeAffinities typeAffinity)
    {
        _typeAffinity = typeAffinity;

        return this;
    }

    internal Column Build()
    {
        return new Column(_columnName, _columnIsPrimaryKey, _columnOrdinal, _typeAffinity);
    }
}
