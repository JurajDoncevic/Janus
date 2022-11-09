using Antlr4.Runtime.Misc;
using Janus.Commons.QueryModels;
using Janus.Commons.SelectionExpressions;
using System.Globalization;
using System.Text.RegularExpressions;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.QueryLanguage;
public class QueryLanguageListener : QueryLanguageBaseListener
{
    private QueryModelOpenBuilder? _queryBuilder;
    private HashSet<string> _selectAttributeIds;
    private SelectionExpression _selectionExpression;
    private List<(string fkAttrId, string pkAttrId)> _joins = new();

    public QueryLanguageListener()
    {
        _selectAttributeIds = new HashSet<string>();
        _selectionExpression = TRUE();
    }

    public override void ExitComparison_expr([NotNull] QueryLanguageParser.Comparison_exprContext context)
    {
        base.ExitComparison_expr(context);
    }

    public override void ExitEq_expr([NotNull] QueryLanguageParser.Eq_exprContext context)
    {
        base.ExitEq_expr(context);
    }

    public override void ExitFrom_clause([NotNull] QueryLanguageParser.From_clauseContext context)
    {
        var tableauId = context.TABLEAU_ID().GetText();
        _queryBuilder = QueryModelOpenBuilder.InitOpenQuery(tableauId);
        _queryBuilder = (QueryModelOpenBuilder)_queryBuilder.WithJoining(configuration => _joins.Fold(configuration, (join, conf) => conf.AddJoin(join.fkAttrId, join.pkAttrId)));



        base.ExitFrom_clause(context);
    }

    public override void ExitGte_expr([NotNull] QueryLanguageParser.Gte_exprContext context)
    {
        base.ExitGte_expr(context);
    }

    public override void ExitGt_expr([NotNull] QueryLanguageParser.Gt_exprContext context)
    {
        var gt =
            GT(context.lvalue().ATTRIBUTE_ID().GetText(),
               ParseStringValue(context.rvalue().literal().GetText()));

        base.ExitGt_expr(context);
    }

    public override void ExitJoin_clause([NotNull] QueryLanguageParser.Join_clauseContext context)
    {
        var pkTableauId = context.TABLEAU_ID().GetText().Trim();
        // attr ids in equi join
        var firstAttrId = context.ATTRIBUTE_ID()[0].GetText().Trim();
        var secondAttrId = context.ATTRIBUTE_ID()[1].GetText().Trim();

        (var pkAttrId, var fkAttrId) =
            firstAttrId.StartsWith(pkTableauId)
                ? (firstAttrId, secondAttrId)
                : (secondAttrId, firstAttrId);

        _joins.Add((fkAttrId, pkAttrId));

        base.ExitJoin_clause(context);
    }

    public override void ExitLte_expr([NotNull] QueryLanguageParser.Lte_exprContext context)
    {
        base.ExitLte_expr(context);
    }

    public override void ExitLt_expr([NotNull] QueryLanguageParser.Lt_exprContext context)
    {
        base.ExitLt_expr(context);
    }

    public override void ExitNeq_expr([NotNull] QueryLanguageParser.Neq_exprContext context)
    {
        base.ExitNeq_expr(context);
    }

    public override void ExitProjection_expr([NotNull] QueryLanguageParser.Projection_exprContext context)
    {
        base.ExitProjection_expr(context);
    }

    public override void ExitQuery([NotNull] QueryLanguageParser.QueryContext context)
    {

        _queryBuilder = (QueryModelOpenBuilder?)(_queryBuilder?.WithProjection(conf =>
            _selectAttributeIds.Fold(conf, (attrId, c) => c.AddAttribute(attrId))));

        base.ExitQuery(context);
    }

    public override void ExitSelection_expr([NotNull] QueryLanguageParser.Selection_exprContext context)
    {
        base.ExitSelection_expr(context);
    }

    public override void ExitSelect_clause([NotNull] QueryLanguageParser.Select_clauseContext context)
    {
        _selectAttributeIds = context.projection_expr().STAR_OP() == null
            ? context
              .projection_expr()
              .ATTRIBUTE_ID() // multiple IDs
              .Map(attrId => attrId.GetText())
              .ToHashSet()
            : new HashSet<string>(); // "* in query"
        base.ExitSelect_clause(context);
    }

    public override void ExitWhere_clause([NotNull] QueryLanguageParser.Where_clauseContext context)
    {
        _selectionExpression = ConstructSelectionExpression(context.selection_expr());
        _queryBuilder = (QueryModelOpenBuilder?)(_queryBuilder?.WithSelection(conf => conf.WithExpression(_selectionExpression)));
        base.ExitWhere_clause(context);
    }

    public Result<Query> BuildQuery()
        => _queryBuilder?.Build() ?? Result<Query>.OnFailure();

    private SelectionExpression ConstructSelectionExpression(QueryLanguageParser.Selection_exprContext context)
        => context switch
        {
            var ctx when ctx.NOT_OP() != null => NOT(ConstructSelectionExpression(ctx.selection_expr()[0])),
            var ctx when ctx.AND_OP() != null => AND(ConstructSelectionExpression(ctx.selection_expr()[0]), ConstructSelectionExpression(ctx.selection_expr()[1])),
            var ctx when ctx.OR_OP() != null => OR(ConstructSelectionExpression(ctx.selection_expr()[0]), ConstructSelectionExpression(ctx.selection_expr()[1])),
            var ctx when ctx.comparison_expr() != null => ConstructComparisonOperation(ctx.comparison_expr()),
            var ctx when ctx.BOOLEAN() != null => ctx.BOOLEAN().GetText().ToLower().StartsWith("t") ? TRUE() : FALSE(),
            var ctx when ctx.Start.Type == QueryLanguageLexer.LPAREN => ConstructSelectionExpression(ctx.selection_expr()[0]),
            var ctx => throw new Exception($"Unknown selection expression {ctx.Start.Text}")
        };

    private SelectionExpression ConstructComparisonOperation(QueryLanguageParser.Comparison_exprContext context)
        => context switch
        {
            var ctx when ctx.gt_expr() != null => GT(ctx.gt_expr().lvalue().ATTRIBUTE_ID().GetText(), ConstructLiteralValue(ctx.gt_expr().rvalue().literal())),
            var ctx when ctx.lt_expr() != null => LT(ctx.lt_expr().lvalue().ATTRIBUTE_ID().GetText(), ConstructLiteralValue(ctx.lt_expr().rvalue().literal())),
            var ctx when ctx.gte_expr() != null => GE(ctx.gte_expr().lvalue().ATTRIBUTE_ID().GetText(), ConstructLiteralValue(ctx.gte_expr().rvalue().literal())),
            var ctx when ctx.lte_expr() != null => LE(ctx.lte_expr().lvalue().ATTRIBUTE_ID().GetText(), ConstructLiteralValue(ctx.lte_expr().rvalue().literal())),
            var ctx when ctx.eq_expr() != null => EQ(ctx.eq_expr().lvalue().ATTRIBUTE_ID().GetText(), ConstructLiteralValue(ctx.eq_expr().rvalue().literal())),
            var ctx when ctx.neq_expr() != null => NEQ(ctx.neq_expr().lvalue().ATTRIBUTE_ID().GetText(), ConstructLiteralValue(ctx.neq_expr().rvalue().literal())),
            var ctx => throw new Exception($"Unknown comparison operator in {ctx.GetText()}")
        };

    private object ConstructLiteralValue(QueryLanguageParser.LiteralContext context)
        => context switch
        {
            var ctx when ctx.STRING() != null => ParseStringValue(ctx.GetText().Trim('"')),
            var ctx when ctx.DATETIME() != null => ParseStringValue(ctx.GetText()),
            var ctx when ctx.INTEGER() != null => ParseStringValue(ctx.GetText()),
            var ctx when ctx.DECIMAL() != null => ParseStringValue(ctx.GetText()),
            var ctx when ctx.BINARY() != null => ParseStringValue(ctx.GetText()[2..]),
            var ctx when ctx.BOOLEAN() != null => ParseStringValue(ctx.GetText()),
            var ctx => throw new Exception($"Can't determine literal value type {ctx.GetText()}")
        };

    private object ParseStringValue(string exp)
    {
        if (Regex.IsMatch(exp.Trim(), @"^0|-?[1-9][0-9]*$") && int.TryParse(exp, out var intValue)) // to ignore decimals 
            return intValue;
        if (Regex.IsMatch(exp.Trim(), @"^-?([1-9][0-9]*|0)[\.|,][0-9]+$") && double.TryParse(exp, NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalValue))
            return decimalValue;
        if (bool.TryParse(exp.Trim(), out var boolValue))
            return boolValue;
        if (DateTime.TryParse(exp.Trim(), out var dateTimeValue))
            return dateTimeValue;
        return exp;
    }
}
