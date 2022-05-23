using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.SelectionExpressions
{
    public abstract class Literal : SelectionExpression
    {
        public abstract string LiteralToken { get; }

        public override string ToPrettyString()
            => LiteralToken;

        public override string ToString()
            => LiteralToken;
    }
}
