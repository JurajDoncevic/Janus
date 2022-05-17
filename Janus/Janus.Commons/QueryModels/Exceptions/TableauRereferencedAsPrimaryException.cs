using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.QueryModels.Exceptions
{
    public class TableauRereferencedAsPrimaryException : Exception
    {
        public TableauRereferencedAsPrimaryException(Join join) 
            : base($"Tableau {join.PrimaryKeyTableauId} is re-referenced as a primary key table in new join: {join.ForeignKeyAttributeId}-{join.PrimaryKeyAttributeId}")
        {
        }
    }
}
