namespace Janus.Commons.SelectionExpressions;

public class FalseLiteral : Literal
{
    internal FalseLiteral()
    {
    }

    public override string LiteralToken => "FALSE";
}
