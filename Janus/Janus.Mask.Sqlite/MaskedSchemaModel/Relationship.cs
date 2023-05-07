namespace Janus.Mask.Sqlite.MaskedSchemaModel;
public sealed class Relationship
{
    private readonly string _foreignKeyColumnName;
    private readonly string _primaryKeyColumnName;
    private readonly string _foreignKeyTableName;
    private readonly string _primaryKeyTableName;

    public Relationship(string foreignKeyColumnName, string primaryKeyColumnName, string foreignKeyTableName, string primaryKeyTableName)
    {
        if (string.IsNullOrEmpty(foreignKeyColumnName))
        {
            throw new ArgumentException($"'{nameof(foreignKeyColumnName)}' cannot be null or empty.", nameof(foreignKeyColumnName));
        }

        if (string.IsNullOrEmpty(primaryKeyColumnName))
        {
            throw new ArgumentException($"'{nameof(primaryKeyColumnName)}' cannot be null or empty.", nameof(primaryKeyColumnName));
        }

        if (string.IsNullOrEmpty(foreignKeyTableName))
        {
            throw new ArgumentException($"'{nameof(foreignKeyTableName)}' cannot be null or empty.", nameof(foreignKeyTableName));
        }

        if (string.IsNullOrEmpty(primaryKeyTableName))
        {
            throw new ArgumentException($"'{nameof(primaryKeyTableName)}' cannot be null or empty.", nameof(primaryKeyTableName));
        }

        _foreignKeyColumnName = foreignKeyColumnName;
        _primaryKeyColumnName = primaryKeyColumnName;
        _foreignKeyTableName = foreignKeyTableName;
        _primaryKeyTableName = primaryKeyTableName;
    }

    public string ForeignKeyColumnName => _foreignKeyColumnName;

    public string PrimaryKeyColumnName => _primaryKeyColumnName;

    public string ForeignKeyTableName => _foreignKeyTableName;

    public string PrimaryKeyTableName => _primaryKeyTableName;

    public override bool Equals(object? obj)
    {
        return obj is Relationship relationship &&
               _foreignKeyColumnName.Equals(relationship._foreignKeyColumnName) &&
               _primaryKeyColumnName.Equals(relationship._primaryKeyColumnName) &&
               _foreignKeyTableName.Equals(relationship._foreignKeyTableName) &&
               _primaryKeyTableName.Equals(relationship._primaryKeyTableName);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_foreignKeyColumnName, _primaryKeyColumnName, _foreignKeyTableName, _primaryKeyTableName);
    }
}
