using Janus.Mediation.SchemaMediationModels.MediationQueryModels;

namespace Janus.Mediation.SchemaMediationModels;
/// <summary>
/// Describes a tableau mediation
/// </summary>
public class TableauMediation
{
    private readonly string _tableauName;
    private readonly string _tableauDescription;
    private readonly SourceQuery _sourceQuery;
    private readonly List<AttributeMediation> _attributeMediations;
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="tableauName">Declared mediated tableau name</param>
    /// <param name="tableauDescription">Declared mediated tableau description</param>
    /// <param name="sourceQuery">Source query for the mediated tableau data</param>
    /// <param name="attributeMediations">Attribute mediations within this mediation</param>
    internal TableauMediation(string tableauName, string tableauDescription, SourceQuery sourceQuery, List<AttributeMediation> attributeMediations)
    {
        _tableauName = tableauName;
        _tableauDescription = tableauDescription;
        _sourceQuery = sourceQuery;
        _attributeMediations = attributeMediations;
    }

    /// <summary>
    /// Declared mediated tableau name
    /// </summary>
    public string TableauName => _tableauName;
    /// <summary>
    /// Declared mediated tableau description
    /// </summary>
    public string TableauDescription => _tableauDescription;
    /// <summary>
    /// Source query for the mediated tableau data
    /// </summary>
    public SourceQuery SourceQuery => _sourceQuery;
    /// <summary>
    /// Attribute mediations within this mediation
    /// </summary>
    public IReadOnlyList<AttributeMediation> AttributeMediations => _attributeMediations;

    /// <summary>
    /// Gets the attribute name that was mapped to the given source query attribute id. This attribute id appears in the source query projection clause.
    /// </summary>
    /// <param name="sourceAttributeId">Source attribute id. Attribute declared in the SELECT clause</param>
    /// <returns>String with name of declared mediated attribute</returns>
    public string? GetDeclaredAttributeName(string sourceAttributeId) =>
        _attributeMediations.FirstOrDefault(mediation => mediation.SourceAttributeId.Equals(sourceAttributeId))?.DeclaredAttributeName;

    /// <summary>
    /// Gets the source query attribute id that was mapped to the given attribute name. The source attribute id is found in the source query projection.
    /// </summary>
    /// <param name="declaredAttributeName">Declared mediated attribute name</param>
    /// <returns>String with id of the source attribute</returns>
    public string? GetSourceAttributeId(string declaredAttributeName) =>
        _attributeMediations.FirstOrDefault(mediation => mediation.DeclaredAttributeName.Equals(declaredAttributeName))?.SourceAttributeId;
}
