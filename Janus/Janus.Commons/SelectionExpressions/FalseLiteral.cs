using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.SelectionExpressions;

public class FalseLiteral : Literal
{
    internal FalseLiteral()
    {
    }

    public override string LiteralToken => "FALSE";
}
