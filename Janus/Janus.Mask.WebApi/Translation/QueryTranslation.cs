using FunctionalExtensions.Base;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Commons.SelectionExpressions;
using System.Globalization;
using System.Text.RegularExpressions;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.Mask.WebApi.Translation;
public static class QueryTranslation
{
    public static Query TranslateQuery(TableauId onTableauId, string? selection = null)
    {
        var selectionExpression = TranslateSelection(selection?.TrimStart('?'), $"{onTableauId.ToString()}.");

        var queryBuilder = QueryModelOpenBuilder.InitOpenQuery(onTableauId)
                            .WithSelection(conf => conf.WithExpression(selectionExpression));

        return queryBuilder.Build();
    }

    private static SelectionExpression TranslateSelection(string? selectionString, string? attributeNamePrefix = "")
    {
        if (!string.IsNullOrWhiteSpace(selectionString))
        {
            return ParseSelection(selectionString, attributeNamePrefix);
        }
        else
        {
            return TRUE();
        }
    }

    private static ComparisonOperator ParseComparison(string comparisonText, string? attributeNamePrefix = "")
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

private static SelectionExpression ParseSelection(string selectionString, string? attributeNamePrefix = "")
    => !selectionString.Contains('&')
        ? ParseComparison(selectionString, attributeNamePrefix)
        : selectionString.Split("&")
            .Map(compString => ParseComparison(compString, attributeNamePrefix))
            .Fold((SelectionExpression)TRUE(), (comp1, expr) => AND(comp1, expr));

private static object GetTypedValue(string valueString)
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
