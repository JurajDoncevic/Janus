namespace Janus.Mediator.WebApp.ViewModels;

public class SchemaMediationViewModel
{
    public string SchemaMediationScript { get; set; }
    /// <summary>
    /// Loaded schemas
    /// </summary>
    public Dictionary<RemotePointViewModel, DataSourceViewModel> LoadedDataSourceSchemas { get; set; }
    /// <summary>
    /// Schemas available for loading - don't contain a ds with the same name as a loaded schema ds
    /// </summary>
    public List<RemotePointViewModel> AvailableRemotePoints { get; set; }

    public OperationOutcomeViewModel? OperationOutcome { get; set; } = null;
}
