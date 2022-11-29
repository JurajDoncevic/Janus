using Janus.Commons.SchemaModels;

namespace Janus.Commons.QueryModels.Exceptions;

public class AttributeDoesNotExistException : Exception
{
    internal AttributeDoesNotExistException(AttributeId attributeId, string dataSourceName)
        : base($"Attribute with ID {attributeId} does not exist in data source {dataSourceName}")
    {
    }
}
