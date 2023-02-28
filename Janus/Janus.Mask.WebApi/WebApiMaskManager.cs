using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.SchemaModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Logging;
using Janus.Mask.Persistence;
using Janus.Mask.WebApi.InstanceManagement.Typing;
using Janus.Mask.WebApi.LocalCommanding;
using Janus.Mask.WebApi.LocalDataModel;
using Janus.Mask.WebApi.LocalQuerying;
using Janus.Mask.WebApi.Translation;
using JanusGenericMask.InstanceManagement.Web;

namespace Janus.Mask.WebApi;
public sealed class WebApiMaskManager 
    : MaskManager<WebApiQuery, TableauId, string?, Unit, Unit, WebApiDelete, WebApiInsert, WebApiUpdate, object, object, IEnumerable<ControllerTyping>>
{
    private readonly WebApiInstance _webApiInstance;
    private readonly WebApiQueryTranslator _queryTranslator;
    private readonly WebApiCommandTranslator _commandTranslator;

    public WebApiMaskManager(MaskCommunicationNode communicationNode,
                             WebApiMaskQueryManager queryManager,
                             WebApiMaskCommandManager commandManager,
                             WebApiMaskSchemaManager schemaManager,
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
        _queryTranslator = queryTranslator;
        _commandTranslator = commandTranslator;
        _webApiInstance = new WebApiInstance(maskOptions.WebApiOptions, commandManager, queryManager, schemaManager, logger);
    }

    public bool IsInstanceRunning => _webApiInstance.IsRunning();

    /// <summary>
    /// Starts the Web API instance
    /// </summary>
    /// <returns></returns>
    public Result StartWebApi()
        => Results.AsResult(() =>
        {
            return _webApiInstance.StartApplication(GetCurrentSchema());
        });

    /// <summary>
    /// Stops the Web API instance
    /// </summary>
    /// <returns></returns>
    public Result StopWebApi()
        => Results.AsResult(() =>
        {
            return _webApiInstance.StopApplication();
        });

    #region WEB API QUERYING AND COMMANDING

    /// <summary>
    /// Runs a query on the current Web API schema
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="query">Web API query</param>
    /// <returns>Web API DTO data</returns>
    public async Task<Result<WebApiDtoData<TModel>>> RunQuery<TModel>(WebApiQuery query)
        => await Results.AsResult(() =>
        {
            var queryResult =
                _queryManager.RunQuery<TModel>(query)
                .Map(data => (WebApiDtoData<TModel>)data);

            return queryResult;
        });

    /// <summary>
    /// Runs a delete command on the current Web API schema
    /// </summary>
    /// <param name="command">Web API delete command</param>
    /// <returns>Operation outcome</returns>
    public async Task<Result> RunCommand(WebApiDelete command)
        => await Results.AsResult(() =>
        {
            var result =
                _commandManager.RunCommand(command);

            return result;
        });

    /// <summary>
    /// Runs an insert command on the current Web API schema
    /// </summary>
    /// <param name="command">Web API insert command</param>
    /// <returns>Operation outcome</returns>
    public async Task<Result> RunCommand(WebApiInsert command)
        => await Results.AsResult(() =>
        {
            var result =
                _commandManager.RunCommand(command);

            return result;
        });

    /// <summary>
    /// Runs an update command on the current Web API schema
    /// </summary>
    /// <param name="command">Web API update command</param>
    /// <returns>Operation outcome</returns>
    public async Task<Result> RunCommand(WebApiUpdate command)
        => await Results.AsResult(() =>
        {
            var result =
                _commandManager.RunCommand(command);

            return result;
        });
    #endregion
}
