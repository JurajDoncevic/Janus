using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Communication.Remotes;
using Janus.Logging;
using Sharprompt;

namespace Janus.Mediator.ConsoleApp.Displays;
public class SchemaInferrenceSelectionDisplay : BaseDisplay
{
    private readonly ILogger<SchemaInferrenceSelectionDisplay>? _logger;
    public SchemaInferrenceSelectionDisplay(MediatorManager MediatorManager, ILogger? logger) : base(MediatorManager)
    {
        _logger = logger?.ResolveLogger<SchemaInferrenceSelectionDisplay>();
    }

    public override string Title => "REMOTE POINTS FOR SCHEMA INFERRENCE";

    protected async override Task<Result> Display()
        => await Results.AsResult(async () =>
        {
            var selectedRemotePoints =
                Prompt.MultiSelect<RemotePoint>(conf =>
                {
                    conf.Message = "Select remote points to use for schema inferrence";
                    conf.Items = _mediatorController.GetRegisteredRemotePoints();
                    conf.Minimum = 0;
                    conf.TextSelector = rp => rp.ToString();
                    conf.DefaultValues = _mediatorController.LoadedSchemaRemotePoints;
                });


            _mediatorController.UnloadAllSchemas();

            var overallResult =
                selectedRemotePoints.Count() > 0
                ? (await Task.WhenAll(selectedRemotePoints.Select(_mediatorController.LoadSchemaFrom)))
                    .Fold(Results.OnSuccess(), 
                        (res1, res2) => 
                            res1.IsSuccess && res2.IsSuccess
                                ? Results.OnSuccess(res1.Message + "\n" + res2.Message)
                                : Results.OnFailure(res1.Message + "\n" + res2.Message))
                : Results.OnSuccess();
            return await Task.FromResult(overallResult);
        });
}
