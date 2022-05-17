using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.QueryModels.Exceptions
{
    public class JoinsNotConnectedException : Exception
    {
        public JoinsNotConnectedException() 
            : base("Given joins don't create an unified tableau - joins don't create a connected join graph.")
        {
        }
    }
}
