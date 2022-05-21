using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.SelectionExpressions;

/// <summary>
/// Describes a lesser than comparison
/// </summary>
public sealed class LesserOrEqualThan : ComparisonOperation
{
    internal LesserOrEqualThan(string attributeId, object value) : base(attributeId, value)
    {
    }

    public override string OperatorString => "<=";

    public override string ToString()
        => $"{AttributeId} {OperatorString} {Value.ToString()}";
}
