using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.SchemaModels.Exceptions;
public class UpdateSetEmptyException : Exception
{
    internal UpdateSetEmptyException()
    : base($"Update set can't be empty")
    {

    }
}
