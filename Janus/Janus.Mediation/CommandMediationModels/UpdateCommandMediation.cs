using Janus.Commons.CommandModels;
using Janus.Commons.SchemaModels;
using Janus.Mediation.SchemaMediationModels;

namespace Janus.Mediation.CommandMediationModels;

/// <summary>
/// Update command mediation information class
/// </summary>
public sealed class UpdateCommandMediation
{
    private readonly UpdateCommand _localizedUpdateCommand;
    private readonly DataSourceId _targetDataSourceId;
    private readonly UpdateCommand _originalUpdateCommand;
    private readonly DataSourceMediation _dataSourceMediation;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="localizedUpdateCommand">Localized update command</param>
    /// <param name="targetDataSourceId">Data source id of the localized update command targets</param>
    /// <param name="originalUpdateCommand">Original update command over a mediated data source</param>
    /// <param name="dataSourceMediation">Data source mediation that created the mediated data source</param>
    /// <exception cref="ArgumentNullException"></exception>
    public UpdateCommandMediation(UpdateCommand localizedUpdateCommand, DataSourceId targetDataSourceId, UpdateCommand originalUpdateCommand, DataSourceMediation dataSourceMediation)
    {
        _localizedUpdateCommand = localizedUpdateCommand ?? throw new ArgumentNullException(nameof(localizedUpdateCommand));
        _targetDataSourceId = targetDataSourceId ?? throw new ArgumentNullException(nameof(targetDataSourceId));
        _originalUpdateCommand = originalUpdateCommand ?? throw new ArgumentNullException(nameof(originalUpdateCommand));
        _dataSourceMediation = dataSourceMediation ?? throw new ArgumentNullException(nameof(dataSourceMediation));
    }

    /// <summary>
    /// Localized update command
    /// </summary>
    public UpdateCommand LocalizedUpdateCommand => _localizedUpdateCommand;

    /// <summary>
    /// Data source id of the data source targeted by the localized update command
    /// </summary>
    public DataSourceId TargetDataSourceId => _targetDataSourceId;

    /// <summary>
    /// Original update command over the mediated data source
    /// </summary>
    public UpdateCommand OriginalUpdateCommand => _originalUpdateCommand;

    /// <summary>
    /// Data source mediation that created the mediated data source
    /// </summary>
    public DataSourceMediation DataSourceMediation => _dataSourceMediation;
}