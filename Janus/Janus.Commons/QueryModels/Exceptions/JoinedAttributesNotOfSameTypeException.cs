using Janus.Commons.SchemaModels;

namespace Janus.Commons.QueryModels.Exceptions;

public class JoinedAttributesNotOfSameTypeException : Exception
{
    public JoinedAttributesNotOfSameTypeException(AttributeId fkAttributeId, DataTypes fkAttributeDataType, AttributeId pkAttributeId, DataTypes pkAttributeDataType)
        : base($"Joined attributes must be of same data type. {fkAttributeId} is of type {fkAttributeDataType}. {pkAttributeId} is of type {pkAttributeDataType}")
    {
    }
}