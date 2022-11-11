namespace Janus.Mediation.SchemaMediationModels.Exceptions;

public class DuplicateAttributeAssignedToProjectionException : Exception
{
    internal DuplicateAttributeAssignedToProjectionException(string attributeId)
        : base($"Attribute {attributeId} already added to the query's projection.")
    {
    }
}
