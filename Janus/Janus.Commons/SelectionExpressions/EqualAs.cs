using Janus.Commons.SchemaModels;

namespace Janus.Commons.SelectionExpressions;

/// <summary>
/// Describes an equals comparison
/// </summary>
public sealed class EqualAs : ComparisonOperator
{
    internal EqualAs(AttributeId attributeId, object value) : base(attributeId, value)
    {
    }

    public override string OperatorString => "EQ";

    public override string PrettyOperatorString => "==";

    protected override HashSet<DataTypes> _compatibleDataTypes => new() { DataTypes.INT, DataTypes.DECIMAL, DataTypes.BINARY, DataTypes.DATETIME, DataTypes.BOOLEAN, DataTypes.STRING };
}
