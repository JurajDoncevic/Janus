namespace Janus.Mediation.MediationModels;
/// <summary>
/// Describes an attribute mediation
/// </summary>
public class AttributeMediation
{
    private readonly Option<string> _attributeDescription;
    private readonly string _sourceAttributeId;
    private readonly string _declaredAttributeName;

    /*
    // possible later updates to consider type conversions with LENSES
    private readonly DataType _expectedDataType;
    private readonly bool _expectedNullability;
    private readonly bool _expectedIsIdentity;
    */

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="sourceAttributeId">Attribute id of the source attribute</param>
    /// <param name="declaredAttributeName">Declared name of the mediated attribute</param>
    /// <param name="attributeDescription">Optional description of the mediated attribute</param>
    public AttributeMediation(string sourceAttributeId, string declaredAttributeName, Option<string> attributeDescription)
    {
        _sourceAttributeId = sourceAttributeId;
        _declaredAttributeName = declaredAttributeName;
        _attributeDescription = attributeDescription;
    }
    
    /// <summary>
    /// Attribute id of the source attribute
    /// </summary>
    public string SourceAttributeId => _sourceAttributeId;
    /// <summary>
    /// Declared name of the mediated attribute
    /// </summary>
    public string DeclaredAttributeName => _declaredAttributeName;
    /// <summary>
    /// Optional mediated attribute description
    /// </summary>
    public Option<string> AttributeDescription => _attributeDescription;
}
