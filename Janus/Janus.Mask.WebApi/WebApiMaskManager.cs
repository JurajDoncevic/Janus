using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.DataModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Components.Translation;
using Janus.Logging;
using Janus.Mask.Persistence;
using Janus.Mask.WebApi.Lenses;
using Janus.Mask.WebApi.LocalCommanding;
using Janus.Mask.WebApi.LocalQuerying;
using Janus.Mask.WebApi.Translation;
using JanusGenericMask.InstanceManagement.Web;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Runtime.CompilerServices;

namespace Janus.Mask.WebApi;
public class WebApiMaskManager : MaskManager
{
    private readonly WebApiInstance _webApiInstance;
    private readonly WebApiQueryTranslator _queryTranslator;
    private readonly WebApiCommandTranslator _commandTranslator;

    public WebApiMaskManager(MaskCommunicationNode communicationNode,
                             WebApiMaskQueryManager queryManager,
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
        _queryTranslator = queryTranslator;
        _commandTranslator = commandTranslator;
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

    #region WEB API QUERYING AND COMMANDING

    public async Task<Result<IEnumerable<TModel>>> RunQuery<TModel>(WebApiQuery query)
        => await Results.AsResult(() =>
        {

            var lens = new TabularDataObjectLens<TModel>(query.StartingWith.ToString() + ".");

            var queryResult =
                Task.FromResult(_queryTranslator.Translate(query)) // this should be pushed into the QueryManager
                .Bind(query => _queryManager.RunQuery(query))
                .Map(data => lens.Get(data));

            return queryResult;
        });

    public async Task<Result> RunCommand(WebApiDelete command)
        => await Results.AsResult(() =>
        {
            var result =
                Task.FromResult(_commandTranslator
                    .TranslateDelete(command)) // this should be pushed into the CommandManager
                    .Bind(translatedCommand => _commandManager.RunCommand(translatedCommand));

            return result;
        });

    public async Task<Result> RunCommand(WebApiInsert command)
        => await Results.AsResult(() =>
        {
            var result =
                Task.FromResult(_commandTranslator
                    .TranslateInsert(command)) // this should be pushed into the CommandManager
                    .Bind(translatedCommand => _commandManager.RunCommand(translatedCommand));

            return result;
        });

    public async Task<Result> RunCommand(WebApiUpdate command)
        => await Results.AsResult(() =>
        {
            var result =
                Task.FromResult(_commandTranslator
                    .TranslateUpdate(command)) // this should be pushed into the CommandManager
                    .Bind(translatedCommand => _commandManager.RunCommand(translatedCommand));
        
            return result;
        });
    #endregion
}
