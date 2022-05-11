using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.QueryModels.Exceptions
{
    public class TableauNotInReferencedTableausException : Exception
    {
        internal TableauNotInReferencedTableausException(string tableauId)
            : base($"Tableau {tableauId} not found in the query's referenced tableaus. Try constructing the query by adding joins first.")
        {
        }
    }
}
