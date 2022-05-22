﻿using Janus.Commons.SchemaModels;
using Janus.Commons.SelectionExpressions;
using System.Runtime.Serialization;

namespace Janus.Commons.QueryModels.Exceptions
{
    public class IncompatibleDataTypeComparisonException : Exception
    {

        public IncompatibleDataTypeComparisonException(string attributeId, DataTypes dataType, ComparisonOperation compareOp)
            : base($"Comparison '{compareOp.ToString()}' is incompatible on attribute {attributeId} with type {dataType}")
        {

        }
    }
}