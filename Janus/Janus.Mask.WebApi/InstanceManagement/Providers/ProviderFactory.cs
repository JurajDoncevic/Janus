using Janus.Commons.SchemaModels;
using Janus.Mask.WebApi.InstanceManagement.Templates;
using Janus.Mask.WebApi.Translation;

namespace Janus.Mask.WebApi.InstanceManagement.Providers;
public class ProviderFactory
{
    private readonly MaskCommandManager _commandManager;
    private readonly MaskQueryManager _queryManager;
    private readonly WebApiQueryTranslator _queryTranslator;
    private readonly WebApiCommandTranslator _commandTranslator;

    public ProviderFactory(MaskCommandManager commandManager, MaskQueryManager queryManager, WebApiQueryTranslator queryTranslator, WebApiCommandTranslator commandTranslator)
    {
        _commandManager = commandManager;
        _queryManager = queryManager;
        _queryTranslator = queryTranslator;
        _commandTranslator = commandTranslator;
    }

    public QueryProvider<TId, TModel> ResolveQueryProvider<TId, TModel>(TableauId targetTableauId, AttributeId identityAttributeId)
        where TModel : BaseDto
        => new QueryProvider<TId, TModel>(targetTableauId, identityAttributeId, _queryManager, _queryTranslator);

    public CommandProvider<TId> ResolveCommandProvider<TId>(TableauId targetTableauId, AttributeId identityAttributeId)
        => new CommandProvider<TId>(targetTableauId, identityAttributeId, _commandManager, _commandTranslator);
}
