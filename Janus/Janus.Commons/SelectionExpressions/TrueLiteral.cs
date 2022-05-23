using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Janus.Commons.SelectionExpressions
{
    public class TrueLiteral : Literal
    {
        internal TrueLiteral()
        {
        }

        public override string LiteralToken => "TRUE";   
    }
}
