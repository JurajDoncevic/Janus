using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.CommandModels;
using Janus.Commons.SelectionExpressions;
using Janus.Mask.Translation;
using Janus.Mask.WebApi.Lenses;
using Janus.Mask.WebApi.MaskedCommandModel;
using Janus.Mask.WebApi.MaskedDataModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.Mask.WebApi.Translation;

public sealed class WebApiCommandTranslator : IMaskCommandTranslator<WebApiDelete, WebApiInsert, WebApiUpdate, string?, object, object>
{
    private readonly WebApiDataTranslator<object> _dataTranslator;

    public WebApiCommandTranslator()
    {
        _dataTranslator = new WebApiDataTranslator<object>();
    }

    public Result<DeleteCommand> TranslateDelete(WebApiDelete delete)
        => Results.AsResult(() =>
        {
            var deleteCommand = TranslateSelection(Option<string>.Some(delete.Selection?.Trim('?')), $"{delete.Target}.")
                .Map(selection =>
                    DeleteCommandOpenBuilder.InitOpenDelete(delete.Target)
                        .WithSelection(conf => conf.WithExpression(selection))
                        .Build());


            return deleteCommand;
        });

    public Result<InsertCommand> TranslateInsert(WebApiInsert insert)
        => Results.AsResult(() =>
        {
            var instantiationData = new WebApiDtoData<object>(new List<object> { insert.Instantiation });
            var dataTranslation = _dataTranslator.Translate(instantiationData);
            if (!dataTranslation)
            {
                return Results.OnFailure<InsertCommand>($"Failed instantiation data translation: {dataTranslation.Message}");
            }

            var insertBuilder = InsertCommandOpenBuilder.InitOpenInsert(insert.Target)
                                    .WithInstantiation(conf => conf.WithValues(dataTranslation.Data));

            return insertBuilder.Build();
        });

    public Result<Instantiation> TranslateInstantiation(Option<object> instantiation)
        => Results.AsResult(() =>
        {
            return Results.OnFailure<Instantiation>("Instantiation translation not implemented. Use the TranslateInsert method.");
        });

    public Result<Mutation> TranslateMutation(Option<object> mutation)
        => Results.AsResult(() =>
        {
            return Results.OnFailure<Mutation>("Mutation translation not implemented. Use the TranslateUpdate method.");
        });

    public Result<SelectionExpression> TranslateSelection(Option<string> selection)
        => TranslateSelection(selection, null);

    public Result<SelectionExpression> TranslateSelection(Option<string> selection, string? attributeNamePrefix = "")
        => Results.AsResult(() =>
        {
            if (selection && !string.IsNullOrWhiteSpace(selection.Value))
            {
                return ParseSelection(selection.Value, attributeNamePrefix);
            }
            else
            {
                return FALSE();
            }
        });

    public Result<UpdateCommand> TranslateUpdate(WebApiUpdate update)
        => Results.AsResult(() =>
        {
            var mutationData = new WebApiDtoData<object>(new List<object> { update.Mutation });
            var dataTranslation = _dataTranslator.Translate(mutationData);
            if (!dataTranslation)
            {
                return Results.OnFailure<UpdateCommand>($"Failed instantiation data translation: {dataTranslation.Message}");
            }

            var mutationValues = new Dictionary<string, object?>(dataTranslation.Data.RowData.FirstOrDefault()?.ColumnValues ?? new Dictionary<string, object?>());

            var updateCommand =
                TranslateSelection(Option<string>.Some(update.Selection?.Trim('?')), $"{update.Target}.")
                    .Map(selection => UpdateCommandOpenBuilder.InitOpenUpdate(update.Target)
                                        .WithMutation(conf => conf.WithValues(mutationValues))
                                        .WithSelection(conf => conf.WithExpression(selection))
                                        .Build());
            return updateCommand;
        });


    private ComparisonOperator ParseComparison(string comparisonText, string? attributeNamePrefix = "")
    => comparisonText switch
    {
        string text when text.Contains("!=") => text.Split("!=").Identity()
                                                    .Map(split => (property: attributeNamePrefix + split[0], value: GetTypedValue(split[1])))
                                                    .Map(t => NEQ(t.property, t.value))
                                                    .Data,
        string text when text.Contains("=") => text.Split('=').Identity()
                                                .Map(split => (property: attributeNamePrefix + split[0], value: GetTypedValue(split[1])))
                                                .Map(t => EQ(t.property, t.value))
                                                .Data,
        string text when text.Contains(">") => text.Split('>').Identity()
                                                .Map(split => (property: attributeNamePrefix + split[0], value: GetTypedValue(split[1])))
                                                .Map(t => GT(t.property, t.value))
                                                .Data,
        string text when text.Contains("<") => text.Split('<').Identity()
                                                .Map(split => (property: attributeNamePrefix + split[0], value: GetTypedValue(split[1])))
                                                .Map(t => LT(t.property, t.value))
                                                .Data,
    };

    private SelectionExpression ParseSelection(string selectionString, string? attributeNamePrefix = "")
        => !selectionString.Contains('&')
            ? ParseComparison(selectionString, attributeNamePrefix)
            : selectionString.Split("&")
                .Map(compString => ParseComparison(compString, attributeNamePrefix))
                .Fold((SelectionExpression)TRUE(), (comp1, expr) => AND(comp1, expr));

    private object GetTypedValue(string valueString)
    {
        if (Regex.IsMatch(valueString.Trim(), @"^0|-?[1-9][0-9]*$") && int.TryParse(valueString, out var longValue)) // to ignore decimals 
            return longValue;
        if (Regex.IsMatch(valueString.Trim(), @"^0|-?[1-9][0-9]*$") && int.TryParse(valueString, out var intValue)) // to ignore decimals 
            return intValue;
        if (Regex.IsMatch(valueString.Trim(), @"^-?([1-9][0-9]*|0)[\.|,][0-9]+$") && double.TryParse(valueString, NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalValue))
            return decimalValue;
        if (bool.TryParse(valueString.Trim(), out var boolValue))
            return boolValue;
        if (DateTime.TryParse(valueString.Trim(), out var dateTimeValue))
            return dateTimeValue;
        return valueString;
    }
}