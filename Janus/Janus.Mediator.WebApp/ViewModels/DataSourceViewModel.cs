using System.ComponentModel.DataAnnotations;

namespace Janus.Mediator.WebApp.ViewModels;

public sealed class DataSourceViewModel
{
    [Display(Name = "Data source JSON")]
    public string DataSourceJson { get; init; } = "{}";
    [Display(Name = "Outcome message")]
    public string Message { get; init; } = string.Empty;
}
