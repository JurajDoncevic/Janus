using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.SelectionExpressions;

public abstract class SelectionExpression
{
    /// <summary>
    /// String representation of the current selection expression
    /// </summary>
    /// <returns></returns>
    public abstract override string ToString();
}
