﻿namespace Janus.Mediator.WebApp.ViewModels;

public sealed class CommandingViewModel
{
    public Option<OperationOutcomeViewModel> OperationOutcome { get; init; }
    public string MediatedDataSourceJson { get; init; } = "{}";
    public string CommandText { get; set; } = string.Empty;
}
