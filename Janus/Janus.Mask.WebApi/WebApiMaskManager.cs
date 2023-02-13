using FunctionalExtensions.Base.Resulting;
using Janus.Communication.Nodes.Implementations;
using Janus.Logging;
using Janus.Mask.Persistence;
using Janus.Mask.WebApi.Translation;
using JanusGenericMask.InstanceManagement.Web;

namespace Janus.Mask.WebApi;
public class WebApiMaskManager : MaskManager
{
    private readonly WebApiInstance _webApiInstance;

    public WebApiMaskManager(MaskCommunicationNode communicationNode,
                             MaskQueryManager queryManager,
                             MaskCommandManager commandManager,
                             MaskSchemaManager schemaManager,
                             MaskPersistenceProvider persistenceProvider,
                             WebApiMaskOptions maskOptions,
                             WebApiQueryTranslator queryTranslator,
                             WebApiCommandTranslator commandTranslator,
                             ILogger? logger = null) 
        : base(communicationNode,
               queryManager,
               commandManager,
               schemaManager,
               persistenceProvider,
               maskOptions,
               logger)
    {
        _webApiInstance = new WebApiInstance(maskOptions.WebApiOptions, commandManager, queryManager, queryTranslator, commandTranslator, logger);
    }

    public bool IsInstanceRunning => _webApiInstance.IsRunning();

    public Result StartWebApi()
        => Results.AsResult(() =>
        {
            return _webApiInstance.StartApplication(GetCurrentSchema());
        });

    public Result StopWebApi()
        => Results.AsResult(() =>
        {
            return _webApiInstance.StopApplication();
        });
}
