using Janus.Commons.SchemaModels;

namespace Janus.Mediation.SchemaMediationModels.Exceptions;

public class AttributeDoesNotExistException : Exception
{
    internal AttributeDoesNotExistException(AttributeId attributeId, IEnumerable<string> dataSourceNames)
        : base($"Attribute with ID {attributeId} does not exist in data sources {string.Join(", ", dataSourceNames)}")
    {
    }
}
