namespace Janus.Components.Translation;

/// <summary>
/// Describes a schema translator
/// </summary>
/// <typeparam name="TSchemaSource">Source schema type</typeparam>
/// <typeparam name="TSchemaDestination">Destination schema type</typeparam>
public interface ISchemaTranslator<TSchemaSource, TSchemaDestination>
{
    /// <summary>
    /// Translates a source schema to a destination type schema
    /// </summary>
    /// <param name="source">Source schema</param>
    /// <returns>Destination schema</returns>
    TSchemaDestination Translate(TSchemaSource source);
}
