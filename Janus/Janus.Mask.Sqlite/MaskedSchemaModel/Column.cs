﻿namespace Janus.Mask.Sqlite.MaskedSchemaModel;
public sealed class Column
{
    private readonly string _name;
    private readonly bool _isPrimaryKey;
    private readonly bool _isNullable;
    private readonly int _ordinal;
    private readonly TypeAffinities _typeAffinity;

    public Column(string name, bool isPrimaryKey, bool isNullable, int ordinal, TypeAffinities typeAffinity)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
        }

        _name = name;
        _isPrimaryKey = isPrimaryKey;
        _ordinal = ordinal;
        _typeAffinity = typeAffinity;
        _isNullable = isNullable;
    }

    public string Name => _name;

    public int Ordinal => _ordinal;

    public TypeAffinities TypeAffinity => _typeAffinity;

    public bool IsPrimaryKey => _isPrimaryKey;

    public bool IsNullable => _isNullable;
}
