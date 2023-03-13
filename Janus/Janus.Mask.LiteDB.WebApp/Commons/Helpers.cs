using FunctionalExtensions.Base;
using Janus.Mask.LiteDB.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text;

namespace Janus.Mask.LiteDB.WebApp.Commons;

internal static class Helpers
{
    internal static Option<OperationOutcomeViewModel> ToOperationOutcomeViewModel(this ITempDataDictionary tempData)
        => tempData.ContainsKey("Constants.IsSuccess") &&
           tempData.ContainsKey("Constants.Message")
           ? Option<OperationOutcomeViewModel>.Some(new OperationOutcomeViewModel
           {
               IsSuccess = (bool)(tempData["Constants.IsSuccess"] ?? false),
               Message = (string)(tempData["Constants.Message"] ?? string.Empty)
           })
           : Option<OperationOutcomeViewModel>.None;

    internal static string PrettyJsonString(string json)
    {
        StringBuilder sb = new StringBuilder();
        bool inString = false;
        int indentLevel = 0;

        foreach (char c in json)
        {
            switch (c)
            {
                case '{':
                case '[':
                    sb.Append(c);
                    sb.AppendLine();
                    indentLevel++;
                    sb.Append(new string(' ', indentLevel * 2));
                    break;
                case '}':
                case ']':
                    sb.AppendLine();
                    indentLevel--;
                    sb.Append(new string(' ', indentLevel * 2));
                    sb.Append(c);
                    break;
                case '"':
                    inString = !inString;
                    sb.Append(c);
                    break;
                case ',':
                    sb.Append(c);
                    if (!inString)
                    {
                        sb.AppendLine();
                        sb.Append(new string(' ', indentLevel * 2));
                    }
                    break;
                case ':':
                    sb.Append(c);
                    if (!inString)
                    {
                        sb.Append(" ");
                    }
                    break;
                default:
                    sb.Append(c);
                    break;
            }
        }

        return sb.ToString();
    }
}
