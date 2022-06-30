using Janus.Commons.SchemaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Janus.Wrapper.CsvFiles;
public static class Utils
{
    public static DataTypes InferAttributeDataType(string value)
    {
        if (Regex.IsMatch(value.Trim(), @"^0|-?[1-9][0-9]*$") && int.TryParse(value, out _))
            return DataTypes.INT;
        if (Regex.IsMatch(value.Trim(), @"^-?([1-9][0-9]*|0)[\.|,][0-9]+$") && double.TryParse(value, out _))
            return DataTypes.DECIMAL;
        if (bool.TryParse(value.Trim(), out _))
            return DataTypes.BOOLEAN;
        if (DateTime.TryParse(value.Trim(), out _))
            return DataTypes.DATETIME;
        return DataTypes.STRING;
    }

    public static object InferAttributeType(string value)
    {
        if (Regex.IsMatch(value.Trim(), @"^0|-?[1-9][0-9]*$") && int.TryParse(value, out int intValue))
            return intValue;
        if (Regex.IsMatch(value.Trim(), @"^-?([1-9][0-9]*|0)[\.|,][0-9]+$") && double.TryParse(value, out double doubleValue))
            return doubleValue;
        if (bool.TryParse(value.Trim(), out bool boolValue))
            return boolValue;
        if (DateTime.TryParse(value.Trim(), out DateTime datetimeValue))
            return datetimeValue;
        return value;
    }
}
