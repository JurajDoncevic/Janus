using Janus.Commons;
using System.ComponentModel.DataAnnotations;

namespace Janus.Mask.WebApi.WebApp.ViewModels;

public sealed class MaskInfoViewModel
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

    [Display(Name = "Port for the web management application")]
    public int WebPort { get; init; }

    [Display(Name = "Eager startup")]
    public bool EagerStartup { get; init; } = false;

    [Display(Name = "Port for the mask web API")]
    public int WebApiPort { get; init; }

    [Display(Name = "Secure port for the mask web API")]
    public int? WebApiSecurePort { get; init; }

    [Display(Name = "SSL used for the mask web API")]
    public bool IsSSLUsed { get; init; }

    [Display(Name = "Web API mask instance running")]
    public bool IsInstanceRunning { get; init; }
    public Option<OperationOutcomeViewModel> OperationOutcome { get; set; } = Option<OperationOutcomeViewModel>.None;
}
