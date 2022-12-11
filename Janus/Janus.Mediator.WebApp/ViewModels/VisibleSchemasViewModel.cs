﻿using Janus.Communication.Remotes;

namespace Janus.Mediator.WebApp.ViewModels;

public class VisibleSchemasViewModel
{
    public Dictionary<RemotePoint, string> VisibleDataSourcesJsons { get; init; } = new ();
}
