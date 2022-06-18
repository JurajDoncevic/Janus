namespace Janus.Commons.QueryModels.Exceptions;

public class DuplicateAttributeAssignedToProjectionException : Exception
{
    internal DuplicateAttributeAssignedToProjectionException(string attributeId!!)
        : base($"Attribute {attributeId} already added to the query's projection.")
    {
    }
}
