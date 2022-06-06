using Janus.Commons.SchemaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.SelectionExpressions;

/// <summary>
/// Describes a greater or equal comparison
/// </summary>
public sealed class GreaterOrEqualThan : ComparisonOperator
{
    protected override HashSet<DataTypes> _compatibleDataTypes => new() { DataTypes.INT, DataTypes.DECIMAL, DataTypes.DATETIME };
    internal GreaterOrEqualThan(string attributeId, object value) : base(attributeId, value)
    {
    }

    public override string OperatorString => "GE";

    public override string PrettyOperatorString => ">=";
}
