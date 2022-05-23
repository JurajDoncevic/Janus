using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.SelectionExpressions;

public abstract class LogicalBinaryOperation : LogicalOperation
{
    private readonly SelectionExpression _leftOperand;
    private readonly SelectionExpression _rightOperand;

    protected LogicalBinaryOperation(SelectionExpression leftOperand, SelectionExpression rightOperand) : base()
    {
        _leftOperand = leftOperand;
        _rightOperand = rightOperand;
    }

    /// <summary>
    /// Left operand of the binary operation
    /// </summary>
    public SelectionExpression LeftOperand => _leftOperand;

    /// <summary>
    /// Right operand of the binary operation
    /// </summary>
    public SelectionExpression RightOperand => _rightOperand;

    public override string ToPrettyString()
        => $"({LeftOperand} {OperatorString} {RightOperand})";

    public override string ToString()
        => $"{OperatorString}({LeftOperand},{RightOperand})";
}
