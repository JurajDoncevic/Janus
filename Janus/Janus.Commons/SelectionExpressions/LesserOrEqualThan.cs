using Janus.Commons.SchemaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.SelectionExpressions;

/// <summary>
/// Describes a lesser than comparison
/// </summary>
public sealed class LesserOrEqualThan : ComparisonOperator
{
    protected override HashSet<DataTypes> _compatibleDataTypes => new() { DataTypes.INT, DataTypes.DECIMAL, DataTypes.DATETIME };
    internal LesserOrEqualThan(string attributeId, object value) : base(attributeId, value)
    {
    }

    public override string OperatorString => "LE";

    public override string PrettyOperatorString => "<=";
}
