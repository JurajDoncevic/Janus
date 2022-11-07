namespace Janus.Mediation.MediationModels.Building;

/// <summary>
/// Schema mediation builder interface
/// </summary>
public interface ISchemaMediationBuilder
{
    /// <summary>
    /// Sets the declared description for the mediated schema
    /// </summary>
    /// <param name="schemaDescription">Declared mediated schema description</param>
    /// <returns>Current schema mediation builder</returns>
    ISchemaMediationBuilder WithDescription(string? schemaDescription);
    /// <summary>
    /// Adds a tableau mediation to the schema mediation
    /// </summary>
    /// <param name="tableauName">Declared name of the mediated tableau</param>
    /// <param name="configuration">Tableau mediation configuration via <see cref="ITableauMediationBuilder"/></param>
    /// <returns>Current schema mediation builer</returns>
    ISchemaMediationBuilder WithTableau(string tableauName, Func<ITableauMediationBuilder, ITableauMediationBuilder> configuration);
    /// <summary>
    /// Builds the currently setup schema mediation
    /// </summary>
    /// <returns>Schema mediation object</returns>
    SchemaMediation Build();
}

/// <summary>
/// Default schema mediation builder implementation
/// </summary>
internal class SchemaMediationBuilder : ISchemaMediationBuilder
{
    private readonly string _declaredSchemaName;
    private string _schemaDescription;
    private readonly List<TableauMediation> _tableauMediations;

    internal SchemaMediationBuilder(string declaredSchemaName, string? schemaDescription = "")
    {
        if (string.IsNullOrWhiteSpace(declaredSchemaName))
        {
            throw new ArgumentException($"'{nameof(declaredSchemaName)}' cannot be null or whitespace.", nameof(declaredSchemaName));
        }

        _declaredSchemaName = declaredSchemaName;
        _schemaDescription = schemaDescription ?? string.Empty;
        _tableauMediations = new List<TableauMediation>();
    }

    public SchemaMediation Build()
    {
        return new SchemaMediation(_declaredSchemaName, _schemaDescription, _tableauMediations);
    }

    public ISchemaMediationBuilder WithDescription(string? schemaDescription)
    {
        _schemaDescription = schemaDescription ?? _schemaDescription;

        return this;
    }

    public ISchemaMediationBuilder WithTableau(string tableauName, Func<ITableauMediationBuilder, ITableauMediationBuilder> configuration)
    {
        if (string.IsNullOrWhiteSpace(tableauName))
        {
            throw new ArgumentException($"'{nameof(tableauName)}' cannot be null or whitespace.", nameof(tableauName));
        }

        var tableauMediation = configuration(new TableauMediationBuilder(tableauName)).Build();

        _tableauMediations.Add(tableauMediation);

        return this;
    }
}
