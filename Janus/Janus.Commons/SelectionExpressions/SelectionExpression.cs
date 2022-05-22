using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Janus.Commons.SelectionExpressions;

public abstract class SelectionExpression
{
    public override bool Equals(object? obj)
    {
        return obj is SelectionExpression other && other.ToString().Equals(this.ToString());
    }

    /// <summary>
    /// String representation of the current selection expression
    /// </summary>
    /// <returns></returns>
    public abstract override string ToString();

    public override int GetHashCode()
    {
        return HashCode.Combine(ToString());
    }
}
