using Janus.Commons.QueryModels.Exceptions;
using Janus.Commons.SchemaModels;
using Janus.Commons.SelectionExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.CommandModels;

public class CommandSelection
{
    private readonly SelectionExpression _expression;
    public SelectionExpression Expression => _expression;
    internal CommandSelection(SelectionExpression expression)
    {
        _expression = expression;
    }

    public override bool Equals(object? obj)
    {
        return obj is CommandSelection selection &&
               _expression.ToString().Equals(selection._expression.ToString());
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_expression);
    }
}

internal static class CommandSelectionUtils
{
    /// <summary>
    /// Check if all attributes in references
    /// </summary>
    /// <param name="selectionExpression"></param>
    /// <param name="referencableAttributes"></param>
    /// <returns></returns>
    /// <exception cref="AttributeNotInReferencedTableausException"></exception>
    internal static bool CheckAttributeReferences(SelectionExpression selectionExpression, HashSet<string> referencableAttributes)
        => selectionExpression switch
        {
            LogicalUnaryOperation unaryOp => CheckAttributeReferences(unaryOp.Operand, referencableAttributes),
            LogicalBinaryOperation binaryOp => CheckAttributeReferences(binaryOp.LeftOperand, referencableAttributes) && CheckAttributeReferences(binaryOp.RightOperand, referencableAttributes),
            ComparisonOperation compareOp => referencableAttributes.Contains(compareOp.AttributeId) ? true : throw new AttributeNotInReferencedTableausException(compareOp.AttributeId),
            Literal literal => true
        };

    /// <summary>
    /// Check if all comparisons are done over appropriate types 
    /// </summary>
    /// <param name="selectionExpression"></param>
    /// <param name="referencableAttrsTypes"></param>
    /// <returns></returns>
    /// <exception cref="IncompatibleDataTypeComparisonException"></exception>
    internal static bool CheckAttributeTypesOnComparison(SelectionExpression selectionExpression, Dictionary<string, DataTypes> referencableAttrsTypes)
        => selectionExpression switch
        {
            LogicalUnaryOperation unaryOp => CheckAttributeTypesOnComparison(unaryOp.Operand, referencableAttrsTypes),
            LogicalBinaryOperation binaryOp => CheckAttributeTypesOnComparison(binaryOp.LeftOperand, referencableAttrsTypes) && CheckAttributeTypesOnComparison(binaryOp.RightOperand, referencableAttrsTypes),
            ComparisonOperation compareOp => compareOp.IsCompatibleDataType(referencableAttrsTypes[compareOp.AttributeId]) ? true : throw new IncompatibleDataTypeComparisonException(compareOp.AttributeId, referencableAttrsTypes[compareOp.AttributeId], compareOp),
            Literal literal => true
        };
}
