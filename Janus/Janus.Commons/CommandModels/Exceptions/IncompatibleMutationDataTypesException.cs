using Janus.Commons.SchemaModels;
using System.Runtime.Serialization;

namespace Janus.Commons.CommandModels.Exceptions;
public class IncompatibleMutationDataTypesException : Exception
{
    public IncompatibleMutationDataTypesException(string tableauId, string referencedAttribute, DataTypes referencedDataType, DataTypes referencingDataType)
        : base ($"Incompatible types mutated on {tableauId} for attribute {referencedAttribute}. Expected {referencedDataType}, but got {referencingDataType}")
    {
    }

}
