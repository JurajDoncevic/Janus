namespace Janus.Commons.SelectionExpressions;

/// <summary>
/// Describes a logical OR
/// </summary>
public sealed class OrOperator : LogicalBinaryOperator
{
    internal OrOperator(SelectionExpression leftOperand, SelectionExpression rightOperand) : base(leftOperand, rightOperand)
    {
    }

    public override string OperatorString => "OR";
}
