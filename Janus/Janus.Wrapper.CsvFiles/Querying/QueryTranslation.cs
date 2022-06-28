using Janus.Commons.SelectionExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.CsvFiles.Querying;
public static class QueryTranslation
{
    private static HashSet<string> GetAllAttributeIdsInSelection(SelectionExpression selectionExpression)
        => selectionExpression switch
        {
            AndOperator andOperator => GetAllAttributeIdsInSelection(andOperator.LeftOperand).Union(GetAllAttributeIdsInSelection(andOperator.RightOperand)).ToHashSet(),
            OrOperator orOperator => GetAllAttributeIdsInSelection(orOperator.LeftOperand).Union(GetAllAttributeIdsInSelection(orOperator.RightOperand)).ToHashSet(),
            NotOperator notOperator => GetAllAttributeIdsInSelection(notOperator.Operand),
            LesserOrEqualThan lesserOrEqualThan => new HashSet<string> { lesserOrEqualThan.AttributeId },
            LesserThan lesserThan => new HashSet<string> { lesserThan.AttributeId },
            GreaterOrEqualThan greaterOrEqualThan => new HashSet<string> { greaterOrEqualThan.AttributeId },
            GreaterThan greaterThan => new HashSet<string> { greaterThan.AttributeId },
            NotEqualAs notEqualAs => new HashSet<string> { notEqualAs.AttributeId },
            EqualAs equalAs => new HashSet<string> { equalAs.AttributeId },
            _ => new HashSet<string>()
        };

    private static Func<Dictionary<string, object>, bool> TranslateSelectionExpression(SelectionExpression selectionExpression)
    {
        return selectionExpression switch
        {
            AndOperator andOperator => (Dictionary<string, object> args) => TranslateSelectionExpression(andOperator.LeftOperand).Invoke(args) && TranslateSelectionExpression(andOperator.RightOperand).Invoke(args),
            OrOperator orOperator => (Dictionary<string, object> args) => TranslateSelectionExpression(orOperator.LeftOperand).Invoke(args) && TranslateSelectionExpression(orOperator.RightOperand).Invoke(args),
            NotOperator notOperator => (Dictionary<string, object> args) => !TranslateSelectionExpression(notOperator.Operand).Invoke(args),
            LesserOrEqualThan lesserOrEqualThan => (Dictionary<string, object> args) => (double)args[lesserOrEqualThan.AttributeId] <= (double)lesserOrEqualThan.Value,
            LesserThan lesserThan => (Dictionary<string, object> args) => (double)args[lesserThan.AttributeId] < (double)lesserThan.Value,
            GreaterOrEqualThan greaterOrEqualThan => (Dictionary<string, object> args) => (double)args[greaterOrEqualThan.AttributeId] >= (double)greaterOrEqualThan.Value,
            GreaterThan greaterThan => (Dictionary<string, object> args) => (double)args[greaterThan.AttributeId] > (double)greaterThan.Value,
            NotEqualAs notEqualAs => (Dictionary<string, object> args) => args[notEqualAs.AttributeId] != notEqualAs.Value,
            EqualAs equalAs => (Dictionary<string, object> args) => args[equalAs.AttributeId] != equalAs.Value,
            _ => (Dictionary<string, object> args) => true
        };
    }
}
