using Janus.Commons.SelectionExpressions;

namespace Janus.Commons.QueryModels;

/// <summary>
/// Describes a query selection clause
/// </summary>
public class Selection
{
    private readonly SelectionExpression _expression;

    /// <summary>
    /// The selection clause's expression
    /// </summary>
    public SelectionExpression Expression => _expression;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="expression">Selection expression</param>
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
