using Janus.Commons.SelectionExpressions;
using static Janus.Commons.SelectionExpressions.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Janus.Serialization.MongoBson.QueryModels;

/// <summary>
/// Converter for selection expressions. Selections are turned into parseable strings.
/// </summary>
internal class SelectionExpressionConverter
{
    /// <summary>
    /// Parses a selection expression from a string
    /// </summary>
    /// <param name="selectionExpressionString"></param>
    /// <returns></returns>
    internal SelectionExpression? FromStringExpression(string selectionExpressionString)
    {
        var selectionExpression = PrefixSelectionExpressionParsing.ParseSelectionExpression(selectionExpressionString);
        return selectionExpression;
    }

    /// <summary>
    /// Creates a parseable string representation of a selection expression
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    internal string ToStringExpression(SelectionExpression value)
    {
        string stringRepresentation = value.ToString();

        return stringRepresentation;
    }

    private static class PrefixSelectionExpressionParsing
    {
        public static SelectionExpression ParseSelectionExpression(string expressionString)
            => expressionString switch
            {
                string exp when exp.StartsWith("AND") => ParseAND(exp),
                string exp when exp.StartsWith("OR") => ParseOR(exp),
                string exp when exp.StartsWith("NOT") => ParseNOT(exp),
                string exp when exp.StartsWith("GE") => ParseGE(exp),
                string exp when exp.StartsWith("GT") => ParseGT(exp),
                string exp when exp.StartsWith("LT") => ParseLT(exp),
                string exp when exp.StartsWith("LE") => ParseLE(exp),
                string exp when exp.StartsWith("EQ") => ParseEQ(exp),
                string exp when exp.StartsWith("NEQ") => ParseNEQ(exp),
                string exp when exp.StartsWith("TRUE") => ParseTRUE(exp),
                _ => throw new FormatException($"Unknown expression: {expressionString}")
            };

        private static NotEqualAs ParseNEQ(string exp)
        {
            var splits = exp[3..^1].Split(",");

            return Expressions.NEQ(splits[0], Utils.ParseStringValue(splits[1]));
        }

        private static EqualAs ParseEQ(string exp)
        {
            var splits = exp[3..^1].Split(",");

            return Expressions.EQ(splits[0], Utils.ParseStringValue(splits[1]));
        }

        private static NotOperator ParseNOT(string exp)
        {
            var matches = Regex.Matches(exp[4..^1], @"(?:[^,()]+((?:\((?>[^()]+|\((?<open>)|\)(?<-open>))*\)))*)+");
            if (matches.Count != 1)
                throw new FormatException($"Can't parse AND: {exp}");


            return Expressions.NOT(
                ParseSelectionExpression(matches[0].Value)
                );
        }

        private static TrueLiteral ParseTRUE(string exp)
            => exp.Equals(TRUE().LiteralToken)
                ? TRUE()
                : throw new FormatException($"Unknown expression: {exp}");

        private static LesserOrEqualThan ParseLE(string exp)
        {
            var splits = exp[3..^1].Split(",");

            return Expressions.LE(splits[0], Utils.ParseStringValue(splits[1]));
        }

        private static LesserThan ParseLT(string exp)
        {
            var splits = exp[3..^1].Split(",");

            return Expressions.LT(splits[0], Utils.ParseStringValue(splits[1]));
        }

        private static GreaterThan ParseGT(string exp)
        {
            var splits = exp[3..^1].Split(",");

            return Expressions.GT(splits[0], Utils.ParseStringValue(splits[1]));
        }

        private static GreaterOrEqualThan ParseGE(string exp)
        {
            var splits = exp[3..^1].Split(",");

            return Expressions.GE(splits[0], Utils.ParseStringValue(splits[1]));
        }

        private static OrOperator ParseOR(string exp)
        {
            var matches = Regex.Matches(exp[4..^1], @"(?:[^,()]+((?:\((?>[^()]+|\((?<open>)|\)(?<-open>))*\)))*)+");
            if (matches.Count != 2)
                throw new FormatException($"Can't parse OR: {exp}");


            return Expressions.OR(
                ParseSelectionExpression(matches[0].Value),
                ParseSelectionExpression(matches[1].Value)
                );
        }

        private static AndOperator ParseAND(string exp)
        {
            var matches = Regex.Matches(exp[4..^1], @"(?:[^,()]+((?:\((?>[^()]+|\((?<open>)|\)(?<-open>))*\)))*)+");
            if (matches.Count != 2)
                throw new FormatException($"Can't parse AND: {exp}");


            return Expressions.AND(
                ParseSelectionExpression(matches[0].Value),
                ParseSelectionExpression(matches[1].Value)
                );
        }
    }

    private static class Utils
    {
        internal static object ParseStringValue(string exp)
        {
            if (Regex.IsMatch(exp.Trim(), @"^0|-?[1-9][0-9]*$") && int.TryParse(exp, out var intValue)) // to ignore decimals 
                return intValue;
            if (Regex.IsMatch(exp.Trim(), @"^-?([1-9][0-9]*|0)[\.|,][0-9]+$") && double.TryParse(exp, out var decimalValue))
                return decimalValue;
            if (bool.TryParse(exp.Trim(), out var boolValue))
                return boolValue;
            if (DateTime.TryParse(exp.Trim(), out var dateTimeValue))
                return dateTimeValue;
            return exp;
        }
    }
}


