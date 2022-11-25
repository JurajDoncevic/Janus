namespace Janus.Commons.SelectionExpressions;

public sealed class TrueLiteral : Literal
{
    internal TrueLiteral()
    {
    }

    public override string LiteralToken => "TRUE";
}
