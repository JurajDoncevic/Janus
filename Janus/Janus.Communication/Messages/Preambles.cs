using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Communication.Messages
{
    /// <summary>
    /// Preambules for all message types
    /// </summary>
    public static class Preambles
    {
        public const string HELLO_REQUEST = "HELLO_REQ";
        public const string HELLO_RESPONSE = "HELLO_RES";
        public const string BYE_REQUEST = "BYE_REQ";
        public const string BYE_RESPONSE = "BYE_RES";
        public const string SCHEMA_REQUEST = "SCHEMA_REQ";
        public const string SCHEMA_RESPONSE = "SCHEMA_RES";
    }
}
