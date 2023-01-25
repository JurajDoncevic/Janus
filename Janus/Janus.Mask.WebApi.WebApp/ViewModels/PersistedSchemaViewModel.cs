namespace Janus.Mask.WebApi.WebApp.ViewModels;

public class PersistedSchemaViewModel
{
    public string MediatedDataSourceVersion { get; set; } = string.Empty;
    public string MediatedDataSourceJson { get; init; } = string.Empty;
    public string MediationScript { get; init; } = string.Empty;
    public Dictionary<RemotePointViewModel, string> LoadedDataSourceJsons { get; init; } = new();
    public DateTime PersistedOn { get; init; }
}
