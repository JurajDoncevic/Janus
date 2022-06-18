namespace Janus.Commons.CommandModels.Exceptions;

public class MissingInstantiationAttributesException : Exception
{
    public MissingInstantiationAttributesException(List<string> unreferencedAttrs, string tableauId)
        : base($"Attributes {string.Join(",", unreferencedAttrs)} of tableau {tableauId} were not referenced in the instatiation clause")
    {
    }

}