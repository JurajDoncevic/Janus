using Janus.Commons.QueryModels.Exceptions;
using Janus.Commons.SchemaModels;
using Janus.Commons.SelectionExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.CommandModels;

/// <summary>
/// Describes a selection clause used in the command model
/// </summary>
public class CommandSelection
{
    private readonly SelectionExpression _expression;

    /// <summary>
    /// Selection clause expression
    /// </summary>
    public SelectionExpression Expression => _expression;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="expression">Selection expression</param>
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
            LogicalUnaryOperator unaryOp => CheckAttributeReferences(unaryOp.Operand, referencableAttributes),
            LogicalBinaryOperator binaryOp => CheckAttributeReferences(binaryOp.LeftOperand, referencableAttributes) && CheckAttributeReferences(binaryOp.RightOperand, referencableAttributes),
            ComparisonOperator compareOp => referencableAttributes.Contains(compareOp.AttributeId) ? true : throw new AttributeNotInReferencedTableausException(compareOp.AttributeId),
            Literal literal => true,
            _ => throw new Exception($"Unkown operation or expression type {selectionExpression}")
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
            LogicalUnaryOperator unaryOp => CheckAttributeTypesOnComparison(unaryOp.Operand, referencableAttrsTypes),
            LogicalBinaryOperator binaryOp => CheckAttributeTypesOnComparison(binaryOp.LeftOperand, referencableAttrsTypes) && CheckAttributeTypesOnComparison(binaryOp.RightOperand, referencableAttrsTypes),
            ComparisonOperator compareOp => compareOp.IsCompatibleDataType(referencableAttrsTypes[compareOp.AttributeId]) ? true : throw new IncompatibleDataTypeComparisonException(compareOp.AttributeId, referencableAttrsTypes[compareOp.AttributeId], compareOp),
            Literal literal => true,
            _ => throw new Exception($"Unkown operation or expression type {selectionExpression}")
        };
}
