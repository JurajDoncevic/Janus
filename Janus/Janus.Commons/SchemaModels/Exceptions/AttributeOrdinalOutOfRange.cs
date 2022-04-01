
namespace Janus.Commons.SchemaModels.Exceptions;

/// <summary>
/// Exception that is thrown when a ordinal for an attribute is out of range
/// </summary>
public class AttributeOrdinalOutOfRange : Exception
{

    internal AttributeOrdinalOutOfRange(string attributeName!!) 
        : base($"Attribute ordinal set out of range for {attributeName}. Attribute ordinals must have values >= 0")
    {

    }
}
