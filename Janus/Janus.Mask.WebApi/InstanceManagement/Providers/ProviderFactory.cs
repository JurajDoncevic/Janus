using Janus.Commons.SchemaModels;
using Janus.Mask.WebApi.InstanceManagement.Templates;

namespace Janus.Mask.WebApi.InstanceManagement.Providers;
public class ProviderFactory
{
    private readonly WebApiMaskCommandManager _commandManager;
    private readonly WebApiMaskQueryManager _queryManager;

    public ProviderFactory(WebApiMaskCommandManager commandManager, WebApiMaskQueryManager queryManager)
    {
        _commandManager = commandManager;
        _queryManager = queryManager;
    }

    public QueryProvider<TId, TModel> ResolveQueryProvider<TId, TModel>(TableauId targetTableauId, AttributeId identityAttributeId)
        where TModel : BaseDto
        => new QueryProvider<TId, TModel>(targetTableauId, identityAttributeId, _queryManager);

    public CommandProvider<TId> ResolveCommandProvider<TId>(TableauId targetTableauId, AttributeId identityAttributeId)
        => new CommandProvider<TId>(targetTableauId, identityAttributeId, _commandManager);
}
