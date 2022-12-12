namespace Janus.Mediator.WebApp.ViewModels;

public sealed class OperationOutcomeViewModel
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
}
