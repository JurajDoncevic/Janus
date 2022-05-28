using System.Runtime.Serialization;

namespace Janus.Commons.CommandModels.Exceptions;

public class NullGivenForNonNullableAttributeException : Exception
{
    public NullGivenForNonNullableAttributeException(string tableauId, string attributeName)
        : base($"Null value given for attribute {attributeName} in {tableauId}.")
    {
    }
}