using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Results;
using Janus.Commons;
using Janus.Commons.CommandModels;
using Janus.Commons.DataModels;
using Janus.Commons.SelectionExpressions;
using Janus.Wrapper.Sqlite.LocalCommanding;
using Janus.Wrapper.Translation;

namespace Janus.Wrapper.Sqlite.Translation;
public class SqliteCommandTranslator
    : ILocalCommandTranslator<SqliteDelete, SqliteInsert, SqliteUpdate, string, string, string>
{
    public Result<SqliteDelete> TranslateDelete(DeleteCommand delete)
        => TranslateSelection(delete.Selection)
            .Bind<string, SqliteDelete>(selection => new SqliteDelete(LocalizeTableauId(delete.OnTableauId), selection));

    public Result<SqliteInsert> TranslateInsert(InsertCommand insert)
        => TranslateInstantiation(Option<Instantiation>.Some(insert.Instantiation))
            .Bind<string, SqliteInsert>(instantiation => new SqliteInsert(instantiation, LocalizeTableauId(insert.OnTableauId)));

    public Result<string> TranslateMutation(Option<Mutation> mutation)
        => ResultExtensions.AsResult(()
            => mutation
                ? $"SET {ValueUpdatesToSetExpression(mutation.Value.ValueUpdates)}"
                : "SET ");

    private string ValueUpdatesToSetExpression(IReadOnlyDictionary<string, object?> valueUpdates)
        => string.Join(
            ", ",
            valueUpdates.Map(kv => $"{kv.Key} = {MaybeWrapInQuot(kv.Value)}"));


    public Result<string> TranslateInstantiation(Option<Instantiation> instantiation)
        => ResultExtensions.AsResult(()
            => instantiation
                ? $"({string.Join(",", instantiation.Value.TabularData.AttributeNames)}) " +
                  $"VALUES {string.Join(",", instantiation.Value.TabularData.RowData.Map(RowToInstantiationString))}"
                : "DEFAULT VALUES");
    private string RowToInstantiationString(RowData row)
        => $"({string.Join(",", row.AttributeValues.Values.Map(MaybeWrapInQuot))})";


    public Result<string> TranslateSelection(Option<CommandSelection> selection)
        => ResultExtensions.AsResult(()
            => selection
                ? GenerateExpression(selection.Value.Expression)
                : "WHERE false");

    public Result<SqliteUpdate> TranslateUpdate(UpdateCommand update)
        => TranslateSelection(update.Selection)
            .Bind(selection => TranslateMutation(Option<Mutation>.Some(update.Mutation)).Map(mutation => (selection, mutation)))
            .Bind<(string selection, string mutation), SqliteUpdate>(x => new SqliteUpdate(LocalizeTableauId(update.OnTableauId), x.selection, x.mutation));

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

    private string LocalizeTableauId(string tableauId)
    {
        (_, _, string tableauName) = Utils.GetNamesFromTableauId(tableauId);
        return tableauName;
    }

    private string LocalizeAttributeId(string attributeId)
    {
        (_, _, string tableauName, string attributeName) = Utils.GetNamesFromAttributeId(attributeId);
        return $"{tableauName}.{attributeName}";
    }

    private object? MaybeWrapInQuot(object? value)
        => value == null
            ? null
            : value is string
                ? $"\"{value}\""
                : value;
}
