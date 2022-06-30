using Janus.Commons.SelectionExpressions;
using static Janus.Commons.SelectionExpressions.Expressions;
using Janus.Wrapper.Core.Translation;
using Janus.Wrapper.CsvFiles.Querying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.CsvFiles.Translation;
public class CsvFilesQueryTranslator : IWrapperQueryTranslator<Query, Selection, Joining, Projection>
{
    public CsvFilesQueryTranslator()
    {

    }

    public Query Translate(Commons.QueryModels.Query query)
        => new Query(
            ConvertIdToPath(query.OnTableauId),
            TranslateProjection(query.Projection),
            TranslateSelection(query.Selection),
            TranslateJoin(query.Joining)
            );

    public Joining TranslateJoin(Option<Commons.QueryModels.Joining> joining)
        =>  joining
            .Map(js => js.Joins
            .Map(j => new Join(
                            ConvertIdToPath(j.ForeignKeyTableauId),
                            ConvertIdToPath(j.ForeignKeyAttributeId),
                            ConvertIdToPath(j.PrimaryKeyTableauId),
                            ConvertIdToPath(j.PrimaryKeyAttributeId))))
            .Match(joins => new Joining(joins.ToList()), () => new Joining(new List<Join>()));

    public Projection TranslateProjection(Option<Commons.QueryModels.Projection> projection)
        => projection.Map(p => p.IncludedAttributeIds.Map(a => ConvertIdToPath(a)))
                     .Match(attrs => new Projection(attrs.ToList()), () => new Projection(new List<string>()));

    public Selection TranslateSelection(Option<Commons.QueryModels.Selection> selection)
        => selection.Map(s => s.Expression)
                    .Map(exp => new Selection(
                            GetAllAttributeIdsInSelection(exp).Map(id => ConvertIdToPath(id)).ToHashSet(), 
                            exp.Identity().Map(TranslateAttributeIdsInSelection).Map(TranslateSelectionExpression).Data
                        ))
            .Match(s => s, () => new Selection(new HashSet<string>(), dict => true));

    private string ConvertIdToPath(string id)
        => id.Replace(".", "/");

    private HashSet<string> GetAllAttributeIdsInSelection(SelectionExpression selectionExpression)
        => selectionExpression switch
        {
            AndOperator andOperator => GetAllAttributeIdsInSelection(andOperator.LeftOperand).Union(GetAllAttributeIdsInSelection(andOperator.RightOperand)).ToHashSet(),
            OrOperator orOperator => GetAllAttributeIdsInSelection(orOperator.LeftOperand).Union(GetAllAttributeIdsInSelection(orOperator.RightOperand)).ToHashSet(),
            NotOperator notOperator => GetAllAttributeIdsInSelection(notOperator.Operand),
            LesserOrEqualThan lesserOrEqualThan => new HashSet<string> {  lesserOrEqualThan.AttributeId },
            LesserThan lesserThan => new HashSet<string> { lesserThan.AttributeId },
            GreaterOrEqualThan greaterOrEqualThan => new HashSet<string> { greaterOrEqualThan.AttributeId },
            GreaterThan greaterThan => new HashSet<string> { greaterThan.AttributeId },
            NotEqualAs notEqualAs => new HashSet<string> { notEqualAs.AttributeId },
            EqualAs equalAs => new HashSet<string> { equalAs.AttributeId },
            _ => new HashSet<string>()
        };

    private SelectionExpression TranslateAttributeIdsInSelection(SelectionExpression selectionExpression)
        => selectionExpression switch
        {
            AndOperator andOperator => AND(TranslateAttributeIdsInSelection(andOperator.LeftOperand), TranslateAttributeIdsInSelection(andOperator.RightOperand)),
            OrOperator orOperator => OR(TranslateAttributeIdsInSelection(orOperator.LeftOperand), TranslateAttributeIdsInSelection(orOperator.RightOperand)),
            NotOperator notOperator => NOT(TranslateAttributeIdsInSelection(notOperator.Operand)),
            LesserOrEqualThan lesserOrEqualThan => LE(ConvertIdToPath(lesserOrEqualThan.AttributeId), lesserOrEqualThan.Value),
            LesserThan lesserThan => LT(ConvertIdToPath(lesserThan.AttributeId), lesserThan.Value),
            GreaterOrEqualThan greaterOrEqualThan => GE(ConvertIdToPath(greaterOrEqualThan.AttributeId), greaterOrEqualThan.Value),
            GreaterThan greaterThan => GT(ConvertIdToPath(greaterThan.AttributeId), greaterThan.Value),
            NotEqualAs notEqualAs => NEQ(ConvertIdToPath(notEqualAs.AttributeId), notEqualAs.Value),
            EqualAs equalAs => EQ(ConvertIdToPath(equalAs.AttributeId), equalAs.Value),
            SelectionExpression exp => exp
        };

    private Func<Dictionary<string, object>, bool> TranslateSelectionExpression(SelectionExpression selectionExpression)
        => selectionExpression switch
        {
            AndOperator andOperator => (Dictionary<string, object> args) => TranslateSelectionExpression(andOperator.LeftOperand).Invoke(args) && TranslateSelectionExpression(andOperator.RightOperand).Invoke(args),
            OrOperator orOperator => (Dictionary<string, object> args) => TranslateSelectionExpression(orOperator.LeftOperand).Invoke(args) || TranslateSelectionExpression(orOperator.RightOperand).Invoke(args),
            NotOperator notOperator => (Dictionary<string, object> args) => !TranslateSelectionExpression(notOperator.Operand).Invoke(args),
            LesserOrEqualThan lesserOrEqualThan => (Dictionary<string, object> args) => Convert.ToDouble(args[lesserOrEqualThan.AttributeId]) <= Convert.ToDouble(lesserOrEqualThan.Value),
            LesserThan lesserThan => (Dictionary<string, object> args) => Convert.ToDouble(args[lesserThan.AttributeId]) < Convert.ToDouble(lesserThan.Value),
            GreaterOrEqualThan greaterOrEqualThan => (Dictionary<string, object> args) => Convert.ToDouble(args[greaterOrEqualThan.AttributeId]) >= Convert.ToDouble(greaterOrEqualThan.Value),
            GreaterThan greaterThan => (Dictionary<string, object> args) => Convert.ToDouble(args[greaterThan.AttributeId]) > Convert.ToDouble(greaterThan.Value),
            NotEqualAs notEqualAs => (Dictionary<string, object> args) => !args[notEqualAs.AttributeId].Equals(notEqualAs.Value),
            EqualAs equalAs => (Dictionary<string, object> args) => args[equalAs.AttributeId].Equals(equalAs.Value),
            _ => (Dictionary<string, object> args) => true
        };
}
