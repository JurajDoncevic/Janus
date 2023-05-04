﻿namespace Janus.Mediator.WebApp;

internal class WebConfiguration
{
    public int Port { get; init; }
    public string AllowedHttpHost { get; init; } = "http://*";
    public string AllowedHttpsHost { get; init; } = "https://*";
}
