using Janus.Commons.SchemaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.SelectionExpressions
{
    public abstract class ComparisonOperation : SelectionExpression
    {
        private readonly object _value;
        private readonly string _attributeId;

        protected abstract HashSet<DataTypes> _compatibleDataTypes { get; }

        protected ComparisonOperation(string attributeId, object value)
        {
            _value = value;
            _attributeId = attributeId;
        }

        /// <summary>
        /// Can the given data type be used with this comparison?
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public bool IsCompatibleDataType(DataTypes dataType)
            => _compatibleDataTypes.Contains(dataType);

        /// <summary>
        /// Comparison value
        /// </summary>
        public object Value => _value;

        /// <summary>
        /// Compared attribute id
        /// </summary>
        public string AttributeId => _attributeId;

        /// <summary>
        /// String representation of the comparison operator
        /// </summary>
        public abstract string OperatorString { get; }

    }
}
