using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.QueryModels.Exceptions
{
    public class ReferencedTableauDoesNotExistException : Exception
    {
        internal ReferencedTableauDoesNotExistException(string tableauId!!, string dataSourceName!!) 
            : base($"Tableau with ID {tableauId} does not exist in data source {dataSourceName}")
        {
        }
    }
}
