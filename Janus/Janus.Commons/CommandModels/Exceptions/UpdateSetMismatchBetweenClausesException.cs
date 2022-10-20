using Janus.Commons.SchemaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.CommandModels.Exceptions;
internal class UpdateSetMismatchBetweenClausesException : Exception
{
    public UpdateSetMismatchBetweenClausesException(UpdateSet? updateSet1, UpdateSet? updateSet2, string tableauId) 
        : base($"Clauses in a command referenced different update sets on tableau {tableauId}: {updateSet1?.ToString() ?? "()"}, {updateSet2?.ToString() ?? "()"}")
    {
    }
}
