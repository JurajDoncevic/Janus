using Janus.Commons.SelectionExpressions.JsonConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Janus.Commons.SelectionExpressions;

[JsonConverter(typeof(SelectionExpressionJsonConverter))]
public abstract class SelectionExpression
{
    public override bool Equals(object? obj)
    {
        return obj is SelectionExpression other && other.ToString().Equals(this.ToString());
    }

    /// <summary>
    /// String representation of the expression
    /// </summary>
    /// <returns>Prefix format string</returns>
    public abstract override string ToString();

    /// <summary>
    /// String representation of the expression in infix format
    /// </summary>
    /// <returns>Infix format string</returns>
    public abstract string ToPrettyString();

    public override int GetHashCode()
    {
        return HashCode.Combine(ToString());
    }
}
