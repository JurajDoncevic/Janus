using Janus.Mediation.SchemaMediationModels.Exceptions;
using Janus.Mediation.SchemaMediationModels.MediationQueryModels;

namespace Janus.Mediation.SchemaMediationModels.Building;

/// <summary>
/// Tableau mediation builder interface
/// </summary>
public interface ITableauMediationBuilder
{
    /// <summary>
    /// Sets the tableau mediation source query for data
    /// </summary>
    /// <param name="sourceQuery">Source query for tableau data</param>
    /// <returns>Current tableau builder</returns>
    ITableauMediationBuilder WithQuery(SourceQuery sourceQuery);
    /// <summary>
    /// Adds a declared attribute mediation to the tableau mediation
    /// </summary>
    /// <param name="attributeName">Mediated attribute name</param>
    /// <param name="attributeDescription">Mediated attribute description</param>
    /// <returns>Current tableau builder</returns>
    ITableauMediationBuilder WithDeclaredAttribute(string attributeName, string? attributeDescription);
    /// <summary>
    /// Sets the mediated tableau description
    /// </summary>
    /// <param name="tableauDescription">Declared description of the mediated tableau</param>
    /// <returns>Current tableau mediation builder</returns>
    ITableauMediationBuilder WithDescription(string? tableauDescription);
    /// <summary>
    /// Builds the current tableau mediation
    /// </summary>
    /// <returns>Tableau mediation object</returns>
    TableauMediation Build(); 
}

/// <summary>
/// Default tableau mediation builder implementation
/// </summary>
internal class TableauMediationBuilder : ITableauMediationBuilder
{
    private string _tableauName;
    private string _tableauDescription;
    private List<AttributeMediation> _attributeMediations;
    private Dictionary<string, string?> _declaredAttributes; // contains attrName and attrDescription
    private SourceQuery? _sourceQuery;

    internal TableauMediationBuilder(string tableauName, string tableauDescription = "")
    {
        _tableauName = tableauName ?? throw new ArgumentException($"'{nameof(tableauName)}' cannot be null or whitespace.", nameof(tableauName));
        _attributeMediations = new List<AttributeMediation>();
        _declaredAttributes = new Dictionary<string, string?>();
        _tableauDescription = tableauDescription ?? string.Empty;
    }

    public TableauMediation Build()
    {
        if(_sourceQuery == null)
        {
            throw new InvalidOperationException("Source query must be initialized");
        }
        if(_sourceQuery.Projection.IncludedAttributeIds.Count != _declaredAttributes.Count)
        {
            throw new MediationAttributesDontMatchException(_sourceQuery.Projection.IncludedAttributeIds, _declaredAttributes.Keys);
        }

        // Build attribute mediations
        _attributeMediations.Clear();
        foreach (var ((attrName, attrDescr), mappedToAttrId) in _declaredAttributes.Zip(_sourceQuery.Projection.IncludedAttributeIds))
        {
            var attrMediationBuilder = new AttributeMediationBuilder(attrName, mappedToAttrId, attrDescr);

            _attributeMediations.Add(attrMediationBuilder.Build());
        }

        return new TableauMediation(_tableauName, _tableauDescription, _sourceQuery, _attributeMediations);
    }

    public ITableauMediationBuilder WithDeclaredAttribute(string attributeName, string? attributeDescription)
    {
        _declaredAttributes.Add(attributeName, attributeDescription);
        return this;
    }

    public ITableauMediationBuilder WithAttributesNamed(params string[] attributeNames)
    {
        return WithAttributesNamed(attributeNames.ToList());
    }

    public ITableauMediationBuilder WithAttributesNamed(IEnumerable<string> attributeNames)
    {
        _declaredAttributes = attributeNames.ToDictionary(attributeName => attributeName!, attributeName => (string)null);
        return this;
    }

    public ITableauMediationBuilder WithDescription(string? tableauDescription)
    {
        _tableauDescription = tableauDescription ?? _tableauDescription;

        return this;
    }

    public ITableauMediationBuilder WithQuery(SourceQuery sourceQuery)
    {
        _sourceQuery = sourceQuery;

        return this;
    }
}
