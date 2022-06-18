using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Janus.Commons.SelectionExpressions.JsonConversion;

public class SelectionExpressionJsonConverter : JsonConverter<SelectionExpression>
{
    public override SelectionExpression? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var selectionExpressionString = reader.GetString();

        var selectionExpression = InfixSelectionExpressionParsing.ParseSelectionExpression(selectionExpressionString);
        return selectionExpression;
    }

    public override void Write(Utf8JsonWriter writer, SelectionExpression value, JsonSerializerOptions options)
    {
        string stringRepresentation = value.ToString();

        writer.WriteStringValue(stringRepresentation);
    }

    private static class InfixSelectionExpressionParsing
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
            => exp.Equals(new TrueLiteral().LiteralToken)
                ? new TrueLiteral()
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

}


