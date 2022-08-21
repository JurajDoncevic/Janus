namespace Janus.Commons.QueryModels.Exceptions;

public class TableauDoesNotExistException : Exception
{
    internal TableauDoesNotExistException(string tableauId, string dataSourceName)
        : base($"Tableau with ID {tableauId} does not exist in data source {dataSourceName}")
    {
    }
}
