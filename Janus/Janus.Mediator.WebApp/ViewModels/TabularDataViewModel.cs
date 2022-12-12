﻿using Janus.Commons.SchemaModels;

namespace Janus.Mediator.WebApp.ViewModels;

public sealed class TabularDataViewModel
{
    public Dictionary<string, DataTypes> ColumnDataTypes { get; init; } = new();
    public List<Dictionary<string, string>> DataRows { get; init; } = new();
}
