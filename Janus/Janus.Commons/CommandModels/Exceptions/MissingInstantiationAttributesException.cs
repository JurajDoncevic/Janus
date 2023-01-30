using Janus.Commons.SchemaModels;

namespace Janus.Commons.CommandModels.Exceptions;

public class MissingInstantiationAttributesException : Exception
{
    public MissingInstantiationAttributesException(IEnumerable<string> unreferencedAttrs, TableauId tableauId)
        : base($"Attributes {string.Join(", ", unreferencedAttrs)} of tableau {tableauId} were not referenced in the instatiation clause")
    {
    }

}