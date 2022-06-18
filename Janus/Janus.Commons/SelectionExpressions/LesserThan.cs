using Janus.Commons.SchemaModels;

namespace Janus.Commons.SelectionExpressions;

/// <summary>
/// Describes a lesser than comparison
/// </summary>
public sealed class LesserThan : ComparisonOperator
{

    protected override HashSet<DataTypes> _compatibleDataTypes => new() { DataTypes.INT, DataTypes.DECIMAL, DataTypes.DATETIME };
    internal LesserThan(string attributeId, object value) : base(attributeId, value)
    {
    }

    public override string OperatorString => "LT";

    public override string PrettyOperatorString => "<";
}
