namespace Janus.Commons.SelectionExpressions;

public abstract class Literal : SelectionExpression
{
    public abstract string LiteralToken { get; }

    public override string ToPrettyString()
        => LiteralToken;

    public override string ToString()
        => LiteralToken;
}
