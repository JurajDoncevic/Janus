using FunctionalExtensions.Base.Results;
using Janus.Communication.Remotes;
using Janus.Logging;
using Sharprompt;

namespace Janus.Mediator.ConsoleApp.Displays;
public class SchemaInferrenceSelectionDisplay : BaseDisplay
{
    private readonly ILogger<SchemaInferrenceSelectionDisplay>? _logger;
    public SchemaInferrenceSelectionDisplay(MediatorController mediatorController, ILogger? logger) : base(mediatorController)
    {
        _logger = logger?.ResolveLogger<SchemaInferrenceSelectionDisplay>();
    }

    public override string Title => "REMOTE POINTS FOR SCHEMA INFERRENCE";

    protected async override Task<Result> Display()
        => await ResultExtensions.AsResult(async () =>
        {
            var selectedRemotePoints =
                Prompt.MultiSelect<RemotePoint>(conf =>
                {
                    conf.Message = "Select remote points to use for schema inferrence";
                    conf.Items = _mediatorController.GetRegisteredRemotePoints();
                    conf.Minimum = 0;
                    conf.TextSelector = rp => rp.ToString();
                    conf.DefaultValues = _mediatorController.SchemaInferredRemotePoints;
                });


            _mediatorController.ClearRemotePointsFromSchemaInferrence();

            var overallResult =
                selectedRemotePoints.Count() > 0
                ? selectedRemotePoints.Select(_mediatorController.AddRemotePointToSchemaInferrence)
                    .Aggregate(
                        (result1, result2) => result1.IsSuccess && result2.IsSuccess
                                                ? Result.OnSuccess(result1.Message + "\n" + result2.Message)
                                                : Result.OnFailure(result1.Message + "\n" + result2.Message))
                : Result.OnSuccess();
            return await Task.FromResult(overallResult);
        });
}
