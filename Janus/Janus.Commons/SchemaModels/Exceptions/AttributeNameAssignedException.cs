
namespace Janus.Commons.SchemaModels;

/// <summary>
/// Exception that is thrown when an attribute with the same name already exists in a tableau
/// </summary>
public class AttributeNameAssignedException : Exception
{
    internal AttributeNameAssignedException(string attributeName!!, string tableauName!!)
        : base($"Attribute with name {attributeName} already exists in tableau {tableauName}.")
    {

    }
}
