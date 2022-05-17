using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.QueryModels.Exceptions
{
    public class InvalidAttributeIdException : Exception
    {
        public InvalidAttributeIdException(string attributeId) 
            : base($"Invalid attribute id give:{attributeId}. Must contain at least one '.'")
        {
        }
    }
}
