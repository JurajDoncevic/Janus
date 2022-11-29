using Janus.Commons.SchemaModels;

namespace Janus.Commons.SelectionExpressions;

/// <summary>
/// Describes a not equals comparison
/// </summary>
public sealed class NotEqualAs : ComparisonOperator
{

    protected override HashSet<DataTypes> _compatibleDataTypes => new() { DataTypes.INT, DataTypes.DECIMAL, DataTypes.BINARY, DataTypes.DATETIME, DataTypes.BOOLEAN, DataTypes.STRING };

    internal NotEqualAs(AttributeId attributeId, object value) : base(attributeId, value)
    {
    }

    public override string OperatorString => "NEQ";

    public override string PrettyOperatorString => "!=";
}
