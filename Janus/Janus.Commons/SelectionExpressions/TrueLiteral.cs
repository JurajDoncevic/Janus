namespace Janus.Commons.SelectionExpressions;

public class TrueLiteral : Literal
{
    internal TrueLiteral()
    {
    }

    public override string LiteralToken => "TRUE";
}
