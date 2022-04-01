
namespace Janus.Commons.SchemaModels.Exceptions;

/// <summary>
/// Exception that is thrown when a ordinal for an attribute is already assigned to another attribute in a tableau
/// </summary>
public class AttributeOrdinalAssignedException : Exception
{
    internal AttributeOrdinalAssignedException(int ordinalNumber, string attributeName!!, string tableauName!!) 
        : base($"Ordinal {ordinalNumber} can't be assigned to attribute {attributeName}. It is already assigned to an attribute on tableau {tableauName}")
    {
        
    }
}
