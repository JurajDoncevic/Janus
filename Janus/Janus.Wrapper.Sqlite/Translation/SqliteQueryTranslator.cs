using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Commons.SelectionExpressions;
using Janus.Wrapper.Sqlite.LocalQuerying;
using Janus.Wrapper.Translation;

namespace Janus.Wrapper.Sqlite.Translation;
public sealed class SqliteQueryTranslator : ILocalQueryTranslator<SqliteQuery, string, string, string>
{
    public Result<SqliteQuery> Translate(Query query)
        => TranslateSelection(query.Selection)
            .Bind(selection => TranslateJoining(query.Joining, query.OnTableauId).Map(joining => (selection, joining)))
            .Bind(result => TranslateProjection(query.Projection).Map(projection => (result.selection, result.joining, projection)))
            .Map(((string selection, string joining, string projection) result)
                => new SqliteQuery(query.OnTableauId.ToString(), result.selection, result.joining, result.projection));

    public Result<string> TranslateJoining(Option<Joining> joining, TableauId? startingWith)
        => Results.AsResult(
            () => $"FROM {LocalizeTableauId(startingWith ?? joining.Value.Joins.First().ForeignKeyTableauId)}" +
                  joining.Match(
                      j => j.Joins.OrderBy(j => j.ForeignKeyTableauId.Equals(startingWith)).Fold("",
                            (join, expr) => expr + $" LEFT JOIN {LocalizeTableauId(join.PrimaryKeyTableauId)} ON {LocalizeAttributeId(join.PrimaryKeyAttributeId)}={LocalizeAttributeId(join.ForeignKeyAttributeId)}"),
                      () => ""));

    public Result<string> TranslateProjection(Option<Projection> projection)
        => Results.AsResult(
            () => projection
                    ? $"SELECT {string.Join(", ", projection.Value.IncludedAttributeIds.Map(LocalizeAttributeId))}"
                    : "SELECT *");

    public Result<string> TranslateSelection(Option<Selection> selection)
        => "WHERE " + selection.Match(
                sel => GenerateExpression(sel.Expression),
                () => "true");

    private string GenerateExpression(SelectionExpression expression)
        => expression switch
        {
            LogicalUnaryOperator unaryOp => GenerateUnaryOp(unaryOp),
            LogicalBinaryOperator binaryOp => GenerateBinaryOp(binaryOp),
            ComparisonOperator compOp => GenerateComparisonOp(compOp),
            Literal literal => GenerateLiteral(literal),
            _ => throw new Exception($"Unknown expression type {expression}")
        };

    private string GenerateLiteral(Literal literal)
        => literal switch
        {
            TrueLiteral => "true",
            FalseLiteral => "false",
            _ => throw new Exception($"Uknown literal operator {literal.LiteralToken}")
        };

    private string GenerateComparisonOp(ComparisonOperator compOp)
        => compOp switch
        {
            EqualAs eq => $"{LocalizeAttributeId(eq.AttributeId)}={MaybeWrapInQuot(eq.Value)}",
            NotEqualAs neq => $"{LocalizeAttributeId(neq.AttributeId)}<>{MaybeWrapInQuot(neq.Value)}",
            GreaterThan gt => $"{LocalizeAttributeId(gt.AttributeId)}>{MaybeWrapInQuot(gt.Value)}",
            GreaterOrEqualThan gte => $"{LocalizeAttributeId(gte.AttributeId)}>={MaybeWrapInQuot(gte.Value)}",
            LesserThan lt => $"{LocalizeAttributeId(lt.AttributeId)}<{MaybeWrapInQuot(lt.Value)}",
            LesserOrEqualThan lte => $"{LocalizeAttributeId(lte.AttributeId)}<={MaybeWrapInQuot(lte.Value)}",
            _ => throw new Exception($"Uknown comparison operator {compOp.OperatorString}")
        };

    private string GenerateBinaryOp(LogicalBinaryOperator binaryOp)
        => binaryOp switch
        {
            AndOperator andOp => $"({GenerateExpression(andOp.LeftOperand)} AND {GenerateExpression(andOp.RightOperand)})",
            OrOperator orOp => $"({GenerateExpression(orOp.LeftOperand)} OR {GenerateExpression(orOp.RightOperand)})",
            _ => throw new Exception($"Unknown logical binary operator {binaryOp.OperatorString}")
        };

    private string GenerateUnaryOp(LogicalUnaryOperator unaryOp)
        => unaryOp switch
        {
            NotOperator notOp => $"NOT({GenerateExpression(notOp.Operand)})",
            _ => throw new Exception($"Unknown logical binary operator {unaryOp.OperatorString}")
        };

    private string LocalizeTableauId(TableauId tableauId)
    {
        (_, _, string tableauName) = tableauId.NameTuple;
        return tableauName;
    }

    private string LocalizeAttributeId(AttributeId attributeId)
    {
        (_, _, string tableauName, string attributeName) = attributeId.NameTuple;
        return $"{tableauName}.{attributeName}";
    }

    private object MaybeWrapInQuot(object value)
        => value is string
            ? $"\"{value}\""
            : value;

}
