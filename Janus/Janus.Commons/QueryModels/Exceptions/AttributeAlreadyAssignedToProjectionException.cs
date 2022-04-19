using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.QueryModels.Exceptions
{
    public class AttributeAlreadyAssignedToProjectionException : Exception
    {
        internal AttributeAlreadyAssignedToProjectionException(string attributeId!!) 
            : base($"Attribute {attributeId} already added to the query's projection.")
        {
        }
    }
}
