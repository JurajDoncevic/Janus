namespace Janus.Mediation.SchemaMediationModels.Building;
/// <summary>
/// Attribute mediation builder interface
/// </summary>
public interface IAttributeMediationBuilder
{
    /// <summary>
    /// Sets the mediated attribute description 
    /// </summary>
    /// <param name="attributeDescription">Mediated attribute description</param>
    /// <returns>Current attribute mediation builder</returns>
    IAttributeMediationBuilder WithDescription(string attributeDescription);
    /// <summary>
    /// Sets the mediated attribute declared name
    /// </summary>
    /// <param name="attributeName">Mediated attribute declared name</param>
    /// <returns>Current attribute mediation builder</returns>
    IAttributeMediationBuilder WithName(string attributeName);
    /// <summary>
    /// Sets the source query attribute id for the declared attribute
    /// </summary>
    /// <param name="sourceAttributeId">Attribute id from the source query projection clause</param>
    /// <returns>Current attribute mediation builder</returns>
    IAttributeMediationBuilder WithSourceAttributeId(string sourceAttributeId);
    /// <summary>
    /// Builds the currently setup attribute mediation
    /// </summary>
    /// <returns>Attribute mediation object</returns>
    AttributeMediation Build();
}

internal class AttributeMediationBuilder : IAttributeMediationBuilder
{
    private string _attributeName;
    private string? _attributeDescription;
    private string _sourceAttributeId;

    internal AttributeMediationBuilder(string attributeName, string sourceAttributeId, string attributeDescription = "")
    {
        _attributeName = attributeName;
        _attributeDescription = attributeDescription ?? string.Empty;
        _sourceAttributeId = sourceAttributeId;
    }

    public AttributeMediation Build()
    {
        return new AttributeMediation(
            _sourceAttributeId, 
            _attributeName,  
            _attributeDescription is not null 
                ? Option<string>.Some(_attributeDescription!) 
                : Option<string>.None);
    }

    public IAttributeMediationBuilder WithDescription(string? attributeDescription)
    {
        _attributeDescription = attributeDescription;

        return this;
    }

    public IAttributeMediationBuilder WithName(string attributeName)
    {
        _attributeName = attributeName ?? _attributeName;

        return this;
    }

    public IAttributeMediationBuilder WithSourceAttributeId(string sourceAttributeId)
    {
        _sourceAttributeId = sourceAttributeId ?? _attributeName;

        return this;
    }
}