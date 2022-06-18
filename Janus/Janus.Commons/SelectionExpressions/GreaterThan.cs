﻿using Janus.Commons.SchemaModels;

namespace Janus.Commons.SelectionExpressions;

/// <summary>
/// Describes a greater than comparison
/// </summary>
public class GreaterThan : ComparisonOperator
{
    protected override HashSet<DataTypes> _compatibleDataTypes => new() { DataTypes.INT, DataTypes.DECIMAL, DataTypes.DATETIME };
    internal GreaterThan(string attributeId, object value) : base(attributeId, value)
    {
    }

    public override string OperatorString => "GT";

    public override string PrettyOperatorString => ">";
}
