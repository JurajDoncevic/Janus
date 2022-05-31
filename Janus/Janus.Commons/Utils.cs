﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Janus.Commons
{
    internal static class Utils
    {
        internal static (string dataSourceName, string schemaName, string tableauName, string attributeName) GetNamesFromAttributeId(string attributeId)
        {
            var splitNames = attributeId.Split(".", 4, StringSplitOptions.TrimEntries);
            if (splitNames.Length == 4)
                return (
                        splitNames[0],
                        splitNames[1],
                        splitNames[2],
                        splitNames[3]
                    );
            else
                throw new InvalidDataException($"{attributeId} is not a valid attribute id");
        }

        internal static (string dataSourceName, string schemaName, string tableauName) GetNamesFromTableauId(string tableauId)
        {
            var splitNames = tableauId.Split(".", 3, StringSplitOptions.TrimEntries);
            if (splitNames.Length == 3)
                return (
                        splitNames[0],
                        splitNames[1],
                        splitNames[2]
                    );
            else
                throw new InvalidDataException($"{tableauId} is not a valid tableau id");
        }

        internal static object ParseStringValue(string exp)
        {
            if (Regex.IsMatch(exp.Trim(), @"^0|-?[1-9][0-9]*$") && int.TryParse(exp, out var intValue)) // to ignore decimals 
                return intValue;
            if (Regex.IsMatch(exp.Trim(), @"^-?[0-9][1-9]*[\.|,][0-9]+$") && double.TryParse(exp, out var decimalValue))
                return decimalValue;
            if (bool.TryParse(exp.Trim(), out var boolValue))
                return boolValue;
            if (DateTime.TryParse(exp.Trim(), out var dateTimeValue))
                return dateTimeValue;
            return exp;
        }
    }
}
