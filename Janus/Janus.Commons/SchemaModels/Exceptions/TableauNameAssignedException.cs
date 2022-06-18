
namespace Janus.Commons.SchemaModels.Exceptions;

/// <summary>
/// Exception that is thrown when a tableau with the same name already exists in a schema
/// </summary>
public class TableauNameAssignedException : Exception
{
    internal TableauNameAssignedException(string tableauName!!, string schemaName!!)
        : base($"Tableau named {tableauName} already exists in schema {schemaName}.")
    {
    }
}
