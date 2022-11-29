using Janus.Commons.SchemaModels;

namespace Janus.Commons.SelectionExpressions;

/// <summary>
/// Describes a greater or equal comparison
/// </summary>
public sealed class GreaterOrEqualThan : ComparisonOperator
{
    protected override HashSet<DataTypes> _compatibleDataTypes => new() { DataTypes.INT, DataTypes.DECIMAL, DataTypes.DATETIME };
    internal GreaterOrEqualThan(AttributeId attributeId, object value) : base(attributeId, value)
    {
    }

    public override string OperatorString => "GE";

    public override string PrettyOperatorString => ">=";
}
