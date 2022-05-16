using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.QueryModels.Exceptions
{
    public class DuplicateJoinNotSupportedException : Exception
    {
        public DuplicateJoinNotSupportedException(string tableauIdFK, string attributeIdFK, string tableauIdPK, string attributeIdPK) 
            : base($"Duplicate join not supported. Duplicate detected on join of tableaus {tableauIdFK} and {tableauIdPK} with {attributeIdFK} and {attributeIdPK}")
        {
        }
    }
}
