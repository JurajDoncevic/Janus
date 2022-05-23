﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.SelectionExpressions;

/// <summary>
/// Describes a logical AND
/// </summary>
public class AndOperation : LogicalBinaryOperation
{
    internal AndOperation(SelectionExpression leftOperand, SelectionExpression rightOperand) : base(leftOperand, rightOperand)
    {
    }

    public override string OperatorString => "AND";
}