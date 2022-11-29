using Janus.Commons;
using Janus.Commons.SchemaModels;

namespace Janus.Mediation.SchemaMediationModels;

/// <summary>
/// Describes a data source mediation
/// </summary>
public class DataSourceMediation
{
    private readonly string _dataSourceName;
    private readonly string _dataSourceDescription;
    private readonly string _dataSourceVersion;
    private readonly Dictionary<string, DataSource> _availableDataSources;
    private readonly List<SchemaMediation> _schemaMediations;
    private readonly bool _propagateUpdateSets = true;
    private readonly bool _propagateAttributeDescriptions = false;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dataSourceName">Mediated data source declared name</param>
    /// <param name="dataSourceDescription">Mediated data source declared description</param>
    /// <param name="dataSourceVersion">Mediated data source declared version</param>
    /// <param name="propagateUpdateSets">Sources' update sets propagation setting</param>
    /// <param name="propagateAttributeDescriptions">Attribute source description propagation setting</param>
    /// <param name="availableDataSources">Available data sources for this mediation</param>
    /// <param name="schemaMediations">Schema mediations within this mediation</param>
    internal DataSourceMediation(string dataSourceName,
                                 string dataSourceDescription,
                                 string dataSourceVersion,
                                 bool propagateUpdateSets,
                                 bool propagateAttributeDescriptions,
                                 List<DataSource> availableDataSources,
                                 List<SchemaMediation> schemaMediations)
    {
        _dataSourceName = dataSourceName;
        _dataSourceDescription = dataSourceDescription;
        _dataSourceVersion = dataSourceVersion;
        _propagateUpdateSets = propagateUpdateSets;
        _propagateAttributeDescriptions = propagateAttributeDescriptions;
        _availableDataSources = availableDataSources.ToDictionary(x => x.Name, x => x);
        _schemaMediations = schemaMediations;
    }

    /// <summary>
    /// Mediated data source declared name
    /// </summary>
    public string DataSourceName => _dataSourceName;
    /// <summary>
    /// Mediated data source declared description
    /// </summary>
    public string DataSourceDescription => _dataSourceDescription;
    /// <summary>
    /// Mediated data source declared version
    /// </summary>
    public string DataSourceVersion => _dataSourceVersion;
    /// <summary>
    /// Available data sources for this mediation
    /// </summary>
    public IReadOnlyDictionary<string, DataSource> AvailableDataSources => _availableDataSources;
    /// <summary>
    /// Schema mediations within this mediation
    /// </summary>
    public IReadOnlyList<SchemaMediation> SchemaMediations => _schemaMediations;
    /// <summary>
    /// Does this mediation propagate update sets from sources?
    /// </summary>
    public bool PropagateUpdateSets => _propagateUpdateSets;
    /// <summary>
    /// Does this mediation propagate attribute descriptions from sources?
    /// </summary>
    public bool PropagateAttributeDescriptions => _propagateAttributeDescriptions;
    /// <summary>
    /// Gets the schema mediation for the schema declared with given name
    /// </summary>
    /// <param name="schemaName"></param>
    /// <returns>Schema mediation</returns>
    public SchemaMediation? this[string schemaName] => _schemaMediations.SingleOrDefault(sm => sm.SchemaName.Equals(schemaName));

    /// <summary>
    /// Gets the source attribute id for a declared attributeId
    /// </summary>
    /// <param name="declaredAttributeId"></param>
    /// <returns></returns>
    public string? GetSourceAttributeId(string declaredAttributeId)
        => Utils.GetNamesFromAttributeId(declaredAttributeId).Identity()
                .Map(names => this[names.schemaName]![names.tableauName]![names.attributeName]!.SourceAttributeId)
                .Data;

}
