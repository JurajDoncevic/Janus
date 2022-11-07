namespace Janus.Mediation.MediationModels.Exceptions;

public class AttributeNotInReferencedTableausException : Exception
{
    public AttributeNotInReferencedTableausException(string attributeId)
        : base($"Attribute {attributeId} not found in the query's referenced tableaus. Try constructing the query by adding joins first.")
    {
    }
}
