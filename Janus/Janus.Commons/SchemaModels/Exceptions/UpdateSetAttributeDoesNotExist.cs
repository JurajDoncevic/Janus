
namespace Janus.Commons.SchemaModels.Exceptions;

/// <summary>
/// Exception that is thrown when an attribute with the same name already exists in a tableau
/// </summary>
public class UpdateSetAttributeDoesNotExist : Exception
{
    internal UpdateSetAttributeDoesNotExist(string attributeName, string tableauName)
        : base($"Attribute {attributeName} referenced in update set doesn't exist in {tableauName}.")
    {

    }
}
