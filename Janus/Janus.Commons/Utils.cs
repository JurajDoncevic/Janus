using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
