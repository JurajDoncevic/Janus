using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.QueryModels.Exceptions
{
    public class SelfJoinNotSupportedException : Exception
    {
        internal SelfJoinNotSupportedException(string tableauId) 
            : base($"Self join on tableau {tableauId} not supported")
        {
        }
    }
}
