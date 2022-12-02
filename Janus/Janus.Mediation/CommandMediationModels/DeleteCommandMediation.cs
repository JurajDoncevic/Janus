using Janus.Commons.CommandModels;
using Janus.Commons.SchemaModels;
using Janus.Mediation.SchemaMediationModels;

namespace Janus.Mediation.CommandMediationModels;

/// <summary>
/// Delete command mediation information
/// </summary>
public sealed class DeleteCommandMediation
{
    private readonly DeleteCommand _localizedDeleteCommand;
    private readonly DataSourceId _targetDataSourceId;
    private readonly DeleteCommand _originalDeleteCommand;
    private readonly DataSourceMediation _dataSourceMediation;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="localizedDeleteCommand">Localized delete command</param>
    /// <param name="targetDataSourceId">Data source id of the localized delete command</param>
    /// <param name="originalDeleteCommand">Delete command over the mediated data source</param>
    /// <param name="dataSourceMediation">Data source mediation that created the mediated data source</param>
    /// <exception cref="ArgumentNullException"></exception>
    public DeleteCommandMediation(DeleteCommand localizedDeleteCommand, DataSourceId targetDataSourceId, DeleteCommand originalDeleteCommand, DataSourceMediation dataSourceMediation)
    {
        _localizedDeleteCommand = localizedDeleteCommand ?? throw new ArgumentNullException(nameof(localizedDeleteCommand));
        _targetDataSourceId = targetDataSourceId ?? throw new ArgumentNullException(nameof(targetDataSourceId));
        _originalDeleteCommand = originalDeleteCommand ?? throw new ArgumentNullException(nameof(originalDeleteCommand));
        _dataSourceMediation = dataSourceMediation ?? throw new ArgumentNullException(nameof(dataSourceMediation));
    }

    /// <summary>
    /// Localized delete command
    /// </summary>
    public DeleteCommand LocalizedDeleteCommand => _localizedDeleteCommand;

    /// <summary>
    /// Data source id of the data source targeted by the delete command
    /// </summary>
    public DataSourceId TargetDataSourceId => _targetDataSourceId;

    /// <summary>
    /// Original delete command over the mediated data source
    /// </summary>
    public DeleteCommand OriginalDeleteCommand => _originalDeleteCommand;

    /// <summary>
    /// Data source mediation that created the mediated data source
    /// </summary>
    public DataSourceMediation DataSourceMediation => _dataSourceMediation;
}