using Janus.Commons.SchemaModels;

namespace Janus.Commons.CommandModels.Exceptions;
public class IncompatibleInstantiationDataTypesException : Exception
{
    public IncompatibleInstantiationDataTypesException(string tableauId, string referencedAttribute, DataTypes referencedDataType, DataTypes referencingDataType)
        : base($"Incompatible types instantiated on {tableauId} for attribute {referencedAttribute}. Expected {referencedDataType}, but got {referencingDataType}")
    {
    }

}
