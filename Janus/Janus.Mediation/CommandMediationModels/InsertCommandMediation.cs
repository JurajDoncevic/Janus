using Janus.Commons.CommandModels;
using Janus.Commons.SchemaModels;
using Janus.Mediation.SchemaMediationModels;

namespace Janus.Mediation.CommandMediationModels;

/// <summary>
/// Insert command mediation information
/// </summary>
public sealed class InsertCommandMediation
{
    private readonly InsertCommand _localizedInsertCommand;
    private readonly DataSourceId _targetDataSourceId;
    private readonly InsertCommand _originalInsertCommand;
    private readonly DataSourceMediation _dataSourceMediation;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="localizedInsertCommand">Localized insert command</param>
    /// <param name="targetDataSourceId">Datasource id of the targeted data source by the localized insert command</param>
    /// <param name="originalInsertCommand">Original insert command over the mediated data source</param>
    /// <param name="dataSourceMediation">Data source mediation that created the mediated data source</param>
    /// <exception cref="ArgumentNullException"></exception>
    public InsertCommandMediation(InsertCommand localizedInsertCommand, DataSourceId targetDataSourceId, InsertCommand originalInsertCommand, DataSourceMediation dataSourceMediation)
    {
        _localizedInsertCommand = localizedInsertCommand ?? throw new ArgumentNullException(nameof(localizedInsertCommand));
        _targetDataSourceId = targetDataSourceId ?? throw new ArgumentNullException(nameof(targetDataSourceId));
        _originalInsertCommand = originalInsertCommand ?? throw new ArgumentNullException(nameof(originalInsertCommand));
        _dataSourceMediation = dataSourceMediation ?? throw new ArgumentNullException(nameof(dataSourceMediation));
    }

    /// <summary>
    /// Localized insert command
    /// </summary>
    public InsertCommand LocalizedInsertCommand => _localizedInsertCommand;

    /// <summary>
    /// Data source id of the data source targeted by the localized insert command
    /// </summary>
    public DataSourceId TargetDataSourceId => _targetDataSourceId;

    /// <summary>
    /// Original insert command over the mediated data source
    /// </summary>
    public InsertCommand OriginalInsertCommand => _originalInsertCommand;

    /// <summary>
    /// Data source mediation that created the mediated data source
    /// </summary>
    public DataSourceMediation DataSourceMediation => _dataSourceMediation;
}