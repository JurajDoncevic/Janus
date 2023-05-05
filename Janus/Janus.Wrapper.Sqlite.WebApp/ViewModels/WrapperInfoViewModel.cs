using Janus.Commons;
using System.ComponentModel.DataAnnotations;

namespace Janus.Wrapper.Sqlite.WebApp.ViewModels;

public sealed class WrapperInfoViewModel
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

    [Display(Name = "Data source connection string")]
    public string SourceConnectionString { get; init; } = string.Empty;

    [Display(Name = "Allows commands")]
    public bool AllowsCommandExecution { get; init; } = false;

    [Display(Name = "Persistence connection string")]
    public string PersistenceConnectionString { get; init; } = string.Empty;

    [Display(Name = "Port for the web application")]
    public int WebPort { get; init; }

    [Display(Name = "Eager startup")]
    public bool EagerStartup { get; init; } = false;
}
