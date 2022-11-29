using Janus.Commons.SchemaModels;

namespace Janus.Commons.QueryModels.Exceptions;

public class DuplicateAttributeAssignedToProjectionException : Exception
{
    internal DuplicateAttributeAssignedToProjectionException(AttributeId attributeId)
        : base($"Attribute {attributeId} already added to the query's projection.")
    {
    }
}
