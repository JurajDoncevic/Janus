
using Janus.Commons.QueryModels.Exceptions;
using Janus.Commons.SchemaModels;
using Janus.Commons.SelectionExpressions;

namespace Janus.Commons.QueryModels;

/// <summary>
/// Describes a query selection clause
/// </summary>
public class Selection
{
    private readonly SelectionExpression _expression;
    public SelectionExpression Expression => _expression;
    public Selection(SelectionExpression expression)
    {
        _expression = expression;
    }

    public override bool Equals(object? obj)
    {
        return obj is Selection selection &&
               _expression.ToString().Equals(selection._expression.ToString());
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_expression);
    }
}
