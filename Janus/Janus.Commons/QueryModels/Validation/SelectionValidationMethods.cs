using Janus.Commons.QueryModels.Exceptions;
using Janus.Commons.SchemaModels;
using Janus.Commons.SelectionExpressions;

namespace Janus.Commons.QueryModels.Validation
{
    internal static class SelectionValidationMethods
    {
        /// <summary>
        /// Check if all attributes exist in references (referencable attributes)
        /// </summary>
        /// <param name="selectionExpression"></param>
        /// <param name="referencableAttributes"></param>
        /// <returns></returns>
        /// <exception cref="AttributeNotInReferencedTableausException"></exception>
        internal static bool CheckSelectionAttributeReferences(SelectionExpression selectionExpression, HashSet<string> referencableAttributes)
            => selectionExpression switch
            {
                LogicalUnaryOperator unaryOp => CheckSelectionAttributeReferences(unaryOp.Operand, referencableAttributes),
                LogicalBinaryOperator binaryOp => CheckSelectionAttributeReferences(binaryOp.LeftOperand, referencableAttributes) && CheckSelectionAttributeReferences(binaryOp.RightOperand, referencableAttributes),
                ComparisonOperator compareOp => referencableAttributes.Contains(compareOp.AttributeId) ? true : throw new AttributeNotInReferencedTableausException(compareOp.AttributeId),
                Literal literal => true
            };

        /// <summary>
        /// Check if all comparisons are done over appropriate types 
        /// </summary>
        /// <param name="selectionExpression"></param>
        /// <param name="referencableAttrsTypes"></param>
        /// <returns></returns>
        /// <exception cref="IncompatibleDataTypeComparisonException"></exception>
        internal static bool CheckSelectionAttributeTypesOnComparison(SelectionExpression selectionExpression, Dictionary<string, DataTypes> referencableAttrsTypes)
            => selectionExpression switch
            {
                LogicalUnaryOperator unaryOp => CheckSelectionAttributeTypesOnComparison(unaryOp.Operand, referencableAttrsTypes),
                LogicalBinaryOperator binaryOp => CheckSelectionAttributeTypesOnComparison(binaryOp.LeftOperand, referencableAttrsTypes) && CheckSelectionAttributeTypesOnComparison(binaryOp.RightOperand, referencableAttrsTypes),
                ComparisonOperator compareOp => compareOp.IsCompatibleDataType(referencableAttrsTypes[compareOp.AttributeId]) ? true : throw new IncompatibleDataTypeComparisonException(compareOp.AttributeId, referencableAttrsTypes[compareOp.AttributeId], compareOp),
                Literal literal => true
            };
    }
}