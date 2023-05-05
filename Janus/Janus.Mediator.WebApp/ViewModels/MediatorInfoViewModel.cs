using Janus.Commons;
using System.ComponentModel.DataAnnotations;

namespace Janus.Mediator.WebApp.ViewModels;

public sealed class MediatorInfoViewModel
{
    [Display(Name = "Communication node id")]
    public string NodeId { get; init; } = string.Empty;

    [Display(Name = "Communication node listening port")]
    public int ListenPort { get; init; }

    [Display(Name = "Node's communication data format")]
    public CommunicationFormats CommunicationFormat { get; init; }

    [Display(Name = "Node's network adapter type")]
    public NetworkAdapterTypes NetworkAdapterType { get; init; }
    [Display(Name = "Timeout in milliseconds")]
    public int TimeoutMs { get; init; }

    [Display(Name = "Persistence connection string")]
    public string PersistenceConnectionString { get; init; } = string.Empty;

    [Display(Name = "Port for the web application")]
    public int WebPort { get; init; }

    [Display(Name = "Eager startup")]
    public bool EagerStartup { get; init; } = false;
}
