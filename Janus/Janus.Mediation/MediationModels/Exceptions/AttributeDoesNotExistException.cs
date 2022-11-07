namespace Janus.Mediation.MediationModels.Exceptions;

public class AttributeDoesNotExistException : Exception
{
    internal AttributeDoesNotExistException(string attributeId, IEnumerable<string> dataSourceNames)
        : base($"Attribute with ID {attributeId} does not exist in data sources {string.Join(", ", dataSourceNames)}")
    {
    }
}
