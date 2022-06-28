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

}
