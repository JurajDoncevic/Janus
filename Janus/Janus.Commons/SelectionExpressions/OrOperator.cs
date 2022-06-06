using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.SelectionExpressions;

/// <summary>
/// Describes a logical OR
/// </summary>
public class OrOperator : LogicalBinaryOperator
{
    internal OrOperator(SelectionExpression leftOperand, SelectionExpression rightOperand) : base(leftOperand, rightOperand)
    {
    }

    public override string OperatorString => "OR";
}
