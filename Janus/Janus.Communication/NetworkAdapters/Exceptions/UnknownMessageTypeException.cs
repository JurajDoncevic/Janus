using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Communication.NetworkAdapters.Exceptions;

public class UnknownMessageTypeException : Exception
{
    public UnknownMessageTypeException(string? preamble) 
        : base($"Unknown message type received {preamble}")
    {
    }
}
