namespace Janus.Mask.WebApi.WebApp.ViewModels;

public class PersistedSchemaViewModel
{
    public string DataSourceVersion { get; set; } = string.Empty;
    public string DataSourceJson { get; init; } = string.Empty;
    public DateTime PersistedOn { get; init; }
}
