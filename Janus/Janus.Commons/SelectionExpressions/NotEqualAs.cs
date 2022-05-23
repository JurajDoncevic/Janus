using Janus.Commons.SchemaModels;
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

    protected override HashSet<DataTypes> _compatibleDataTypes => new() { DataTypes.INT, DataTypes.DECIMAL, DataTypes.BINARY, DataTypes.DATETIME, DataTypes.BOOLEAN, DataTypes.STRING };
    
    internal NotEqualAs(string attributeId, object value) : base(attributeId, value)
    {
    }

    public override string OperatorString => "NEQ";

    public override string PrettyOperatorString => "!=";
}
