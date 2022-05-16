using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.QueryModels.Exceptions
{
    public class CyclicJoinNotSupportedException : Exception
    {
        public CyclicJoinNotSupportedException(string tableauIdFK, string attributeIdFK, string tableauIdPK, string attributeIdPK) 
            : base($"Cyclic joins are not supported. Join cycle detected adding join between tableaus {tableauIdFK} and {tableauIdPK} with {attributeIdFK} and {attributeIdPK}")
        {
        }
    }
}
