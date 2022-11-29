using Janus.Commons.SchemaModels;

namespace Janus.Commons.QueryModels.Exceptions;

public class PrimaryKeyAttributeNullableException : Exception
{
    public PrimaryKeyAttributeNullableException(AttributeId attributeId)
        : base($"{attributeId} can't be used as primary key attribute in join as it is Nullable")
    {
    }
}