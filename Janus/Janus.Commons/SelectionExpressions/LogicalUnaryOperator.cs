namespace Janus.Commons.SelectionExpressions;

public abstract class LogicalUnaryOperator : LogicalOperator
{
    private readonly SelectionExpression _operand;

    protected LogicalUnaryOperator(SelectionExpression operand) : base()
    {
        _operand = operand;
    }

    public SelectionExpression Operand => _operand;

    public override string ToPrettyString()
        => $"{OperatorString}({Operand})";

    public override string ToString()
        => $"{OperatorString}({Operand})";
}
