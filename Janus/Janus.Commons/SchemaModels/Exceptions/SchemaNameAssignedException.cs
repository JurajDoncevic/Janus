
namespace Janus.Commons.SchemaModels.Exceptions;

/// <summary>
/// Exception that is thrown when a schema with the same name already exists in a data source
/// </summary>
public class SchemaNameAssignedException : Exception
{
    internal SchemaNameAssignedException(string schemaName, string dataSourceName)
        : base($"Schema named {schemaName} already exists in data source {dataSourceName}.")
    {
    }
}
