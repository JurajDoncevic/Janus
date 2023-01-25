using Janus.Communication.Remotes;
using System.ComponentModel.DataAnnotations;

namespace Janus.Mask.WebApi.WebApp.ViewModels;

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

    public override bool Equals(object? obj)
    {
        return obj is RemotePointViewModel model &&
               NodeId == model.NodeId &&
               Address == model.Address &&
               Port == model.Port &&
               RemotePointType == model.RemotePointType;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(NodeId, Address, Port, RemotePointType);
    }

    public string String
        => $"({NodeId};{Address};{Port})";
}
