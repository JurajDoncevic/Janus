﻿namespace Janus.Commons.SelectionExpressions;

/// <summary>
/// Describes a logical AND
/// </summary>
public sealed class AndOperator : LogicalBinaryOperator
{
    internal AndOperator(SelectionExpression leftOperand, SelectionExpression rightOperand) : base(leftOperand, rightOperand)
    {
    }

    public override string OperatorString => "AND";
}
