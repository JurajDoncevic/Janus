
using Janus.Commons.QueryModels.Exceptions;
using Janus.Commons.SchemaModels;
using Janus.Commons.SelectionExpressions;

namespace Janus.Commons.QueryModels;

/// <summary>
/// Describes a query selection clause
/// </summary>
public class Selection
{
    private readonly SelectionExpression _expression;
    public SelectionExpression Expression => _expression;
    public Selection(SelectionExpression expression)
    {
        _expression = expression;
    }

    public override bool Equals(object? obj)
    {
        return obj is Selection selection &&
               _expression == selection._expression;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_expression);
    }
}

public static class SelectionUtils
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
            LogicalUnaryOperation unaryOp => CheckAttributeReferences(unaryOp.Expression, referencableAttributes),
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
            LogicalUnaryOperation unaryOp => CheckAttributeTypesOnComparison(unaryOp.Expression, referencableAttrsTypes),
            LogicalBinaryOperation binaryOp => CheckAttributeTypesOnComparison(binaryOp.LeftOperand, referencableAttrsTypes) && CheckAttributeTypesOnComparison(binaryOp.RightOperand, referencableAttrsTypes),
            ComparisonOperation compareOp => compareOp.IsCompatibleDataType(referencableAttrsTypes[compareOp.AttributeId]) ? true : throw new IncompatibleDataTypeComparisonException(compareOp.AttributeId, referencableAttrsTypes[compareOp.AttributeId], compareOp),
            Literal literal => true
        };
}
