using Janus.Commons.SchemaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.SelectionExpressions;

/// <summary>
/// Describes an equals comparison
/// </summary>
public sealed class EqualAs : ComparisonOperator
{
    internal EqualAs(string attributeId, object value) : base(attributeId, value)
    {
    }

    public override string OperatorString => "EQ";

    public override string PrettyOperatorString => "==";

    protected override HashSet<DataTypes> _compatibleDataTypes => new() { DataTypes.INT, DataTypes.DECIMAL, DataTypes.BINARY, DataTypes.DATETIME, DataTypes.BOOLEAN, DataTypes.STRING };
}
