using Janus.Commons.SchemaModels;

namespace Janus.Mediation.SchemaMediationModels.Exceptions;

public class DuplicateAttributeAssignedToProjectionException : Exception
{
    internal DuplicateAttributeAssignedToProjectionException(AttributeId attributeId)
        : base($"Attribute {attributeId} already added to the query's projection.")
    {
    }
}
