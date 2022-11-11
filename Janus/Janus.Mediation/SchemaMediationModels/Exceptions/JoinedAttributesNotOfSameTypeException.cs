using Janus.Commons.SchemaModels;

namespace Janus.Mediation.SchemaMediationModels.Exceptions;

public class JoinedAttributesNotOfSameTypeException : Exception
{
    public JoinedAttributesNotOfSameTypeException(string fkAttributeId, DataTypes fkAttributeDataType, string pkAttributeId, DataTypes pkAttributeDataType)
        : base($"Joined attributes must be of same data type. {fkAttributeId} is of type {fkAttributeDataType}. {pkAttributeId} is of type {pkAttributeDataType}")
    {
    }
}