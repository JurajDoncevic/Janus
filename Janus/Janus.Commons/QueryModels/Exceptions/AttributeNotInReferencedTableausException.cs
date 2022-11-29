using Janus.Commons.SchemaModels;

namespace Janus.Commons.QueryModels.Exceptions;

public class AttributeNotInReferencedTableausException : Exception
{
    public AttributeNotInReferencedTableausException(AttributeId attributeId)
        : base($"Attribute {attributeId} not found in the query's referenced tableaus. Try constructing the query by adding joins first.")
    {
    }
}
