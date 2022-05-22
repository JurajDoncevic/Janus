using Janus.Commons.SchemaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.SelectionExpressions;

/// <summary>
/// Describes a greater than comparison
/// </summary>
public class GreaterThan : ComparisonOperation
{
    protected override HashSet<DataTypes> _compatibleDataTypes => new() { DataTypes.INT, DataTypes.DECIMAL, DataTypes.DATETIME };
    internal GreaterThan(string attributeId, object value) : base(attributeId, value)
    {
    }

    public override string OperatorString => ">";

    public override string ToString()
        => $"{AttributeId} {OperatorString} {Value.ToString()}";
}
