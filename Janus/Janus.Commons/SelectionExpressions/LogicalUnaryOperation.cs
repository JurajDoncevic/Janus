using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.SelectionExpressions;

public abstract class LogicalUnaryOperation : LogicalOperation
{
    private readonly SelectionExpression _expression;

    protected LogicalUnaryOperation(SelectionExpression expression) : base()
    {
        _expression = expression;
    }

    public SelectionExpression Expression => _expression;
}
