namespace Janus.Components.Translation;
public interface ISchemaTranslator<TSchemaSource, TSchemaDestination>
{
    TSchemaDestination Translate(TSchemaSource source);
}
