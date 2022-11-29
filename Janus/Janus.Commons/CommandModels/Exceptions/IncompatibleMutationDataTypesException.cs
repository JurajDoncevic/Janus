using Janus.Commons.SchemaModels;

namespace Janus.Commons.CommandModels.Exceptions;
public class IncompatibleMutationDataTypesException : Exception
{
    public IncompatibleMutationDataTypesException(TableauId tableauId, string referencedAttribute, DataTypes referencedDataType, DataTypes referencingDataType)
        : base($"Incompatible types mutated on {tableauId} for attribute {referencedAttribute}. Expected {referencedDataType}, but got {referencingDataType}")
    {
    }

}
