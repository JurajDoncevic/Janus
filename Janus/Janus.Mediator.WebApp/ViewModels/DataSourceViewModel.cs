using System.ComponentModel.DataAnnotations;

namespace Janus.Mediator.WebApp.ViewModels;

public sealed class DataSourceViewModel
{
    public string DataSourceJson { get; init; } = "{}";

    public Option<OperationOutcomeViewModel> OperationOutcome { get; init; }
}
