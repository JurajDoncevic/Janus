using Janus.Commons.SchemaModels;

namespace Janus.Commons.QueryModels.Exceptions;

public class TableauDoesNotExistException : Exception
{
    internal TableauDoesNotExistException(TableauId tableauId, string dataSourceName)
        : base($"Tableau with ID {tableauId} does not exist in data source {dataSourceName}")
    {
    }
}
