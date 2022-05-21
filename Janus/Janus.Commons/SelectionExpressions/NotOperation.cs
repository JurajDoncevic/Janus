using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.SelectionExpressions;

/// <summary>
/// Describes a logical NOT
/// </summary>
public sealed class NotOperation : LogicalUnaryOperation
{
    internal NotOperation(SelectionExpression expression) : base(expression)
    {
    }

    public override string OperatorString => "NOT";

    public override string ToString() 
        => $"{OperatorString}({Expression.ToString()})";
}
