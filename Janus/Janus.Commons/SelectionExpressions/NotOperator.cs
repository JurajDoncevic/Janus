namespace Janus.Commons.SelectionExpressions;

/// <summary>
/// Describes a logical NOT
/// </summary>
public sealed class NotOperator : LogicalUnaryOperator
{
    internal NotOperator(SelectionExpression expression) : base(expression)
    {
    }

    public override string OperatorString => "NOT";

    public override string ToString()
        => $"{OperatorString}({Operand.ToString()})";
}
