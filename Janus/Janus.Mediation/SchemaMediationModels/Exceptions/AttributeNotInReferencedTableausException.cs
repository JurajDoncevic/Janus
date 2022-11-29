using Janus.Commons.SchemaModels;

namespace Janus.Mediation.SchemaMediationModels.Exceptions;

public class AttributeNotInReferencedTableausException : Exception
{
    public AttributeNotInReferencedTableausException(AttributeId attributeId)
        : base($"Attribute {attributeId} not found in the query's referenced tableaus. Try constructing the query by adding joins first.")
    {
    }
}
