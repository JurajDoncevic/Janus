﻿using Janus.Commons.SchemaModels;

namespace Janus.Commons.SelectionExpressions;

public abstract class ComparisonOperator : SelectionExpression
{
    private readonly object _value;
    private readonly AttributeId _attributeId;

    protected abstract HashSet<DataTypes> _compatibleDataTypes { get; }

    protected ComparisonOperator(AttributeId attributeId, object value)
    {
        _value = value;
        _attributeId = attributeId;
    }

    /// <summary>
    /// Can the given data type be used with this comparison?
    /// </summary>
    /// <param name="dataType"></param>
    /// <returns></returns>
    public bool IsCompatibleDataType(DataTypes dataType)
        => _compatibleDataTypes.Contains(dataType);

    /// <summary>
    /// Comparison value
    /// </summary>
    public object Value => _value;

    /// <summary>
    /// Compared attribute id
    /// </summary>
    public AttributeId AttributeId => _attributeId;

    /// <summary>
    /// String representation of the comparison operator
    /// </summary>
    public abstract string OperatorString { get; }

    public abstract string PrettyOperatorString { get; }

    public override string ToPrettyString()
        => $"{AttributeId} {OperatorString} {Value.ToString()}";

    public override string ToString()
        => $"{OperatorString}({AttributeId},{Value.ToString()})";

}
