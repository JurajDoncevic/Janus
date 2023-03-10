﻿namespace Janus.Mask.LiteDB.MaskedSchemaModel;
public abstract class Field
{
    protected readonly string _name;
    protected readonly FieldTypes _fieldType;

    protected Field(string name, FieldTypes fieldType)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
        }

        _name = name;
        _fieldType = fieldType;
    }

    public string Name => _name;
    public FieldTypes FieldType => _fieldType;
}
