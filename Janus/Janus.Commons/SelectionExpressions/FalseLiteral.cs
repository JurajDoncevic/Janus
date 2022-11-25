namespace Janus.Commons.SelectionExpressions;

public sealed class FalseLiteral : Literal
{
    internal FalseLiteral()
    {
    }

    public override string LiteralToken => "FALSE";
}
