namespace Janus.Mediation.SchemaMediationModels;

/// <summary>
/// Describes a schema mediation
/// </summary>
public class SchemaMediation
{
    private readonly string _schemaName;
    private readonly string _schemaDescription;
    private readonly List<TableauMediation> _tableauMediations;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="schemaName">Declared name of the mediated schema</param>
    /// <param name="schemaDescription">Description of the mediated schema</param>
    /// <param name="tableauMediations">Tableau mediations within this schema mediation</param>
    internal SchemaMediation(string schemaName, string schemaDescription, List<TableauMediation> tableauMediations)
    {
        _schemaName = schemaName;
        _schemaDescription = schemaDescription;
        _tableauMediations = tableauMediations;
    }

    /// <summary>
    /// Declared name of the mediated schema
    /// </summary>
    public string SchemaName => _schemaName;

    /// <summary>
    /// Description of the mediated schema
    /// </summary>
    public string SchemaDescription => _schemaDescription;

    /// <summary>
    /// Tableau mediations within this schema mediation
    /// </summary>
    public IReadOnlyList<TableauMediation> TableauMediations => _tableauMediations;
}
