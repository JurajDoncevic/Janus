using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.SelectionExpressions;

/// <summary>
/// Describes a logical OR
/// </summary>
public class OrOperation : LogicalBinaryOperation
{
    internal OrOperation(SelectionExpression leftOperand, SelectionExpression rightOperand) : base(leftOperand, rightOperand)
    {
    }

    public override string OperatorString => "OR";

    public override string ToString()
        => $"({LeftOperand.ToString()} {OperatorString} {RightOperand.ToString()})";
}
