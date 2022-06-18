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
