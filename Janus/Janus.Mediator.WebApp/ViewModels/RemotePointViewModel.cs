using Janus.Communication.Remotes;
using System.ComponentModel.DataAnnotations;

namespace Janus.Mediator.WebApp.ViewModels;

public sealed class RemotePointViewModel
{
    [Required]
    [Display(Name = "Node id")]
    public string NodeId { get; init; }
    [Required]
    [Display(Name = "Address")]
    public string Address { get; init; }
    [Required]
    [Display(Name = "Port")]
    public int Port { get; init; }
    public RemotePointTypes RemotePointType { get; init; }
}
