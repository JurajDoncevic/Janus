using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.SelectionExpressions;

public abstract class LogicalOperator : SelectionExpression
{
    /// <summary>
    /// String representation of the operator
    /// </summary>
    public abstract string OperatorString { get; }

    /// <summary>
    /// String representation of the operation instance
    /// </summary>
    /// <returns></returns>
    public abstract override string ToString();
}
