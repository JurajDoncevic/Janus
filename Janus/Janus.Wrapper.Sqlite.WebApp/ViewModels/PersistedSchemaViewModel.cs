namespace Janus.Wrapper.Sqlite.WebApp.ViewModels;

public class PersistedSchemaViewModel
{
    public string InferredDataSourceVersion { get; set; } = string.Empty;
    public string InferredDataSourceJson { get; init; } = string.Empty;
    public DateTime PersistedOn { get; init; }
}
