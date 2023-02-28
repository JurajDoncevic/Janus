using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Commons.SelectionExpressions;
using Janus.Mask.Translation;
using Janus.Mask.WebApi.LocalQuerying;
using System.Globalization;
using System.Text.RegularExpressions;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.Mask.WebApi.Translation;

public sealed class WebApiQueryTranslator : IMaskQueryTranslator<WebApiQuery, TableauId, string?, Unit, Unit>
{
    public Result<Query> Translate(WebApiQuery query)
        => Results.AsResult(() =>
        {
            var translatedQuery =
                TranslateSelection(Option<string?>.Some(query.Selection?.TrimStart('?')), $"{query.StartingWith}.")
                .Map(selectionExpression => QueryModelOpenBuilder.InitOpenQuery(query.StartingWith)
                                            .WithSelection(conf => conf.WithExpression(selectionExpression))
                                            .Build());

            return translatedQuery;
        });

    public Result<Joining> TranslateJoining(Option<Unit> joining, TableauId? startingWith = null)
        => Results.OnFailure<Joining>("Joining translation not implemented. User Translate method.");

    public Result<Projection> TranslateProjection(Option<Unit> projection)
        => Results.OnFailure<Projection>("Projection translation not implemented. User Translate method.");

    public Result<SelectionExpression> TranslateSelection(Option<string?> selection, string? attributeNamePrefix = "")
        => Results.AsResult(() =>
        {
            if (selection && !string.IsNullOrWhiteSpace(selection.Value))
            {
                return ParseSelection(selection.Value, attributeNamePrefix);
            }
            else
            {
                return TRUE();
            }
        });
    public Result<SelectionExpression> TranslateSelection(Option<string?> selection)
        => TranslateSelection(selection, null);

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
