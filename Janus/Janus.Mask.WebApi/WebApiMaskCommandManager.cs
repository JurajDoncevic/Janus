using FunctionalExtensions.Base.Resulting;
using Janus.Communication.Nodes.Implementations;
using Janus.Logging;
using Janus.Mask.WebApi.LocalCommanding;
using Janus.Mask.WebApi.Translation;

namespace Janus.Mask.WebApi;
public sealed class WebApiMaskCommandManager : MaskCommandManager<WebApiDelete, WebApiInsert, WebApiUpdate, string?, object, object>
{
    private readonly WebApiCommandTranslator _commandTranslator;
    private readonly ILogger<WebApiMaskCommandManager>? _logger;
    public WebApiMaskCommandManager(MaskCommunicationNode communicationNode, MaskSchemaManager schemaManager, WebApiCommandTranslator commandTranslator, ILogger? logger = null) : base(communicationNode, schemaManager, commandTranslator, logger)
    {
        _commandTranslator= commandTranslator;
        _logger = logger?.ResolveLogger<WebApiMaskCommandManager>();
    }

    public override async Task<Result> RunCommand(WebApiDelete command)
        => await Results.AsResult(() =>
        {
            var commandResult =
                Task.FromResult(_commandTranslator.TranslateDelete(command))
                .Bind(cmd => RunCommand(cmd));

            return commandResult;
        });

    public override async Task<Result> RunCommand(WebApiInsert command)
        => await Results.AsResult(() =>
        {
            var commandResult =
                Task.FromResult(_commandTranslator.TranslateInsert(command))
                .Bind(cmd => RunCommand(cmd));

            return commandResult;
        });

    public override async Task<Result> RunCommand(WebApiUpdate command)
        => await Results.AsResult(() =>
        {
            var commandResult =
                Task.FromResult(_commandTranslator.TranslateUpdate(command))
                .Bind(cmd => RunCommand(cmd));

            return commandResult;
        });
}
