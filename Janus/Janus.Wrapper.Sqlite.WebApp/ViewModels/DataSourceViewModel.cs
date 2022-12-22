﻿using System.ComponentModel.DataAnnotations;

namespace Janus.Wrapper.Sqlite.WebApp.ViewModels;

public sealed class DataSourceViewModel
{
    public string DataSourceJson { get; init; } = string.Empty;
    public Option<OperationOutcomeViewModel> OperationOutcome { get; init; }

}
