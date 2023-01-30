using Antlr4.Runtime.Misc;
using FunctionalExtensions.Base;
using Janus.Commons.CommandModels;
using Janus.Commons.DataModels;
using Janus.Commons.SchemaModels;
using Janus.Commons.SelectionExpressions;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.CommandLanguage;
public class CommandLanguageListener : CommandLanguageBaseListener
{
    // delete variables
    private DeleteCommandOpenBuilder? _deleteBuilder;

    // update variables
    private UpdateCommandOpenBuilder? _updateBuilder;
    private Dictionary<string, object?> _valueUpdates;

    // insert variables
    private InsertCommandOpenBuilder? _insertBuilder;
    private List<string> _attributeNames;
    private Dictionary<string, DataTypes>? _attributeDataTypes;
    private List<Dictionary<string, object>>? _attributeRowValues;

    // common variables
    private SelectionExpression? _selectionExpression;
    private string? _onTableauId;

    public CommandLanguageListener()
    {
        _selectionExpression = FALSE();
        _attributeNames = new List<string>();
        _attributeDataTypes = new Dictionary<string, DataTypes>();
        _attributeRowValues = new List<Dictionary<string, object>>();
        _valueUpdates = new Dictionary<string, object?>();
    }

    public Option<CommandTypes> ParsedCommandType
        => _deleteBuilder is not null
            ? Option<CommandTypes>.Some(CommandTypes.DELETE)
            : _insertBuilder is not null
                ? Option<CommandTypes>.Some(CommandTypes.INSERT)
                : _updateBuilder is not null
                    ? Option<CommandTypes>.Some(CommandTypes.UPDATE)
                    : Option<CommandTypes>.None;

    public Result<DeleteCommand> BuildDeleteCommand()
        => _deleteBuilder?.Build() ?? Results.OnFailure<DeleteCommand>();
    public Result<InsertCommand> BuildInsertCommand()
        => _insertBuilder?.Build() ?? Results.OnFailure<InsertCommand>();
    public Result<UpdateCommand> BuildUpdateCommand()
        => _updateBuilder?.Build() ?? Results.OnFailure<UpdateCommand>();

    public override void EnterMutation_expr([NotNull] CommandLanguageParser.Mutation_exprContext context)
    {
        base.EnterMutation_expr(context);
    }

    public override void ExitMutation_expr([NotNull] CommandLanguageParser.Mutation_exprContext context)
    {
        var attrName = context.STRUCTURE_NAME().GetText();
        object? mutationValue = ConstructLiteralValue(context.literal());

        _valueUpdates.Add(attrName, mutationValue);
        base.ExitMutation_expr(context);
    }

    public override void ExitAttribute_name_list_expr([NotNull] CommandLanguageParser.Attribute_name_list_exprContext context)
    {
        _attributeNames.AddRange(
            context.STRUCTURE_NAME()
                   .Fold(Enumerable.Empty<string>(), (ctx, attrList) => attrList.Append(ctx.GetText()))
            );


        base.ExitAttribute_name_list_expr(context);
    }

    public override void ExitSelection_expr([NotNull] CommandLanguageParser.Selection_exprContext context)
    {
        base.ExitSelection_expr(context);
    }

    public override void ExitWhere_clause([NotNull] CommandLanguageParser.Where_clauseContext context)
    {
        _selectionExpression = ConstructSelectionExpression(context.selection_expr());
        base.ExitWhere_clause(context);
    }

    public override void ExitFrom_clause([NotNull] CommandLanguageParser.From_clauseContext context)
    {
        base.ExitFrom_clause(context);
    }

    public override void ExitInto_clause([NotNull] CommandLanguageParser.Into_clauseContext context)
    {
        _onTableauId = context.TABLEAU_ID().GetText();

        base.ExitInto_clause(context);
    }

    public override void ExitInstantiation_clause([NotNull] CommandLanguageParser.Instantiation_clauseContext context)
    {
        base.ExitInstantiation_clause(context);
    }

    public override void ExitValues_tuple_expr([NotNull] CommandLanguageParser.Values_tuple_exprContext context)
    {
        if (_attributeDataTypes?.Count == 0)
        {
            var dataTypes =
                context.literal()
                    .Fold(Enumerable.Empty<DataTypes>(), (ctx, dataTypes) => dataTypes.Append(ConstructLiteralDataType(ctx)))
                    .ToList();

            _attributeDataTypes = _attributeNames.Zip(dataTypes).ToDictionary(t => t.First, t => t.Second);

        }

        var attributeValues =
            context.literal()
                .Fold(Enumerable.Empty<object>(), (ctx, values) => values.Append(ConstructLiteralValue(ctx)))
                .Zip(_attributeNames)
                .ToDictionary(t => t.Second, t => t.First);

        _attributeRowValues?.Add(attributeValues);

        base.ExitValues_tuple_expr(context);
    }

    public override void ExitMutation_clause([NotNull] CommandLanguageParser.Mutation_clauseContext context)
    {
        base.ExitMutation_clause(context);
    }

    public override void ExitDelete_command([NotNull] CommandLanguageParser.Delete_commandContext context)
    {
        var tableauId = context.from_clause().TABLEAU_ID().GetText();

        _deleteBuilder = DeleteCommandOpenBuilder.InitOpenDelete(tableauId)
                            .WithSelection(conf => conf.WithExpression(_selectionExpression!));

        base.ExitDelete_command(context);
    }

    public override void EnterInsert_command([NotNull] CommandLanguageParser.Insert_commandContext context)
    {
        _attributeDataTypes?.Clear();
        _attributeNames.Clear();
        _attributeRowValues?.Clear();
        base.EnterInsert_command(context);
    }

    public override void ExitInsert_command([NotNull] CommandLanguageParser.Insert_commandContext context)
    {
        var insertData =
        _attributeRowValues.Fold(TabularDataBuilder.InitTabularData(_attributeDataTypes!),
            (rowData, builder) => builder.AddRow(conf => conf.WithRowData(rowData!)))
            .Build();

        _insertBuilder =
            InsertCommandOpenBuilder.InitOpenInsert(_onTableauId!)
                .WithInstantiation(conf => conf.WithValues(insertData));

        base.ExitInsert_command(context);
    }

    public override void EnterUpdate_command([NotNull] CommandLanguageParser.Update_commandContext context)
    {
        _valueUpdates?.Clear();

        base.EnterUpdate_command(context);
    }

    public override void ExitUpdate_command([NotNull] CommandLanguageParser.Update_commandContext context)
    {
        _onTableauId = context.TABLEAU_ID().GetText();

        _updateBuilder =
        UpdateCommandOpenBuilder.InitOpenUpdate(_onTableauId!)
            .WithMutation(conf => conf.WithValues(_valueUpdates))
            .WithSelection(conf => conf.WithExpression(_selectionExpression!));

        base.ExitUpdate_command(context);
    }

    private SelectionExpression ConstructSelectionExpression(CommandLanguageParser.Selection_exprContext context)
        => context switch
        {
            var ctx when ctx.NOT_OP() != null => NOT(ConstructSelectionExpression(ctx.selection_expr()[0])),
            var ctx when ctx.AND_OP() != null => AND(ConstructSelectionExpression(ctx.selection_expr()[0]), ConstructSelectionExpression(ctx.selection_expr()[1])),
            var ctx when ctx.OR_OP() != null => OR(ConstructSelectionExpression(ctx.selection_expr()[0]), ConstructSelectionExpression(ctx.selection_expr()[1])),
            var ctx when ctx.comparison_expr() != null => ConstructComparisonOperation(ctx.comparison_expr()),
            var ctx when ctx.BOOLEAN() != null => ctx.BOOLEAN().GetText().ToLower().StartsWith("t") ? TRUE() : FALSE(),
            var ctx when ctx.Start.Type == CommandLanguageLexer.LPAREN => ConstructSelectionExpression(ctx.selection_expr()[0]),
            var ctx => throw new Exception($"Unknown selection expression {ctx.Start.Text}")
        };

    private SelectionExpression ConstructComparisonOperation(CommandLanguageParser.Comparison_exprContext context)
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

    private object ConstructLiteralValue(CommandLanguageParser.LiteralContext context)
        => context switch
        {
            var ctx when ctx.STRING() != null => ParseStringValue(ctx.GetText().Trim('"')),
            var ctx when ctx.DATETIME() != null => ParseStringValue(ctx.GetText()),
            var ctx when ctx.INTEGER() != null => ParseStringValue(ctx.GetText()),
            var ctx when ctx.LONGINT() != null => ParseStringValue(ctx.GetText()),
            var ctx when ctx.DECIMAL() != null => ParseStringValue(ctx.GetText()),
            var ctx when ctx.BINARY() != null => ParseStringValue(ctx.GetText()[2..]),
            var ctx when ctx.BOOLEAN() != null => ParseStringValue(ctx.GetText()),
            var ctx => throw new Exception($"Can't determine literal value type {ctx.GetText()}")
        };
    private DataTypes ConstructLiteralDataType(CommandLanguageParser.LiteralContext context)
        => context switch
        {
            var ctx when ctx.STRING() != null => DataTypes.STRING,
            var ctx when ctx.DATETIME() != null => DataTypes.DATETIME,
            var ctx when ctx.INTEGER() != null => DataTypes.INT,
            var ctx when ctx.LONGINT() != null => DataTypes.LONGINT,
            var ctx when ctx.DECIMAL() != null => DataTypes.DECIMAL,
            var ctx when ctx.BINARY() != null => DataTypes.BINARY,
            var ctx when ctx.BOOLEAN() != null => DataTypes.BOOLEAN,
            var ctx => throw new Exception($"Can't determine type of literal {ctx.GetText()}")
        };

    private object ParseStringValue(string exp)
    {
        if (Regex.IsMatch(exp.Trim(), @"^0L|-?[1-9][0-9]*L$") && long.TryParse(exp.Trim().TrimEnd('L'), out var longValue)) // to ignore decimals 
            return longValue;
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
