using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.SchemaModels.Exceptions;
public class InvalidIdStringException : Exception
{
    internal InvalidIdStringException(string givenString) 
        : base($"Invalid identifier string given to construct ID: {givenString}")
    {
    }
}
