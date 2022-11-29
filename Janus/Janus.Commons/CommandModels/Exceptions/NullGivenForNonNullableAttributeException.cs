using Janus.Commons.SchemaModels;

namespace Janus.Commons.CommandModels.Exceptions;

public class NullGivenForNonNullableAttributeException : Exception
{
    public NullGivenForNonNullableAttributeException(TableauId tableauId, string attributeName)
        : base($"Null value given for attribute {attributeName} in {tableauId}.")
    {
    }
}