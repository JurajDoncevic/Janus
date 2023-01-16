using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Commons.SelectionExpressions;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.Mask.WebApi.Translation;
public static class QueryTranslation
{
    public static Query TranslateQuery(TableauId onTableauId, string? selection = null)
    {
        var selectionExpression = TranslateSelection(selection);

        var queryBuilder = QueryModelOpenBuilder.InitOpenQuery(onTableauId)
                            .WithSelection(conf => conf.WithExpression(selectionExpression));

        return queryBuilder.Build();
    }

    private static SelectionExpression TranslateSelection(string? selectionString)
    {
        if(string.IsNullOrWhiteSpace(selectionString))
        {
            return TRUE();
        }
        else
        {
            return TRUE();
        }
    }
}
