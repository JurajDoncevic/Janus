using Janus.Commons.SchemaModels;

namespace Janus.Commons.QueryModels.Exceptions;

public class TableauNotInReferencedTableausException : Exception
{
    internal TableauNotInReferencedTableausException(TableauId tableauId)
        : base($"Tableau {tableauId} not found in the query's referenced tableaus. Try constructing the query by adding joins first.")
    {
    }
}
