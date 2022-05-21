using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.SelectionExpressions;

/// <summary>
/// Describes a not equals comparison
/// </summary>
public sealed class NotEqualAs : ComparisonOperation
{
    internal NotEqualAs(string attributeId, object value) : base(attributeId, value)
    {
    }

    public override string OperatorString => "!=";

    public override string ToString()
        => $"{AttributeId} {OperatorString} {Value.ToString()}";
}
