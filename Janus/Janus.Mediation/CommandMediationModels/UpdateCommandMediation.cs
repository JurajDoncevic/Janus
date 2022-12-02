using Janus.Commons.CommandModels;
using Janus.Commons.SchemaModels;

namespace Janus.Mediation.CommandMediationModels;

public sealed class UpdateCommandMediation
{
    private readonly UpdateCommand _localizedUpdateCommand;
    private readonly DataSourceId _targetDataSourceId;

    public UpdateCommandMediation(UpdateCommand localizedUpdateCommand, DataSourceId targetDataSourceId)
    {
        _localizedUpdateCommand = localizedUpdateCommand;
        _targetDataSourceId = targetDataSourceId;
    }

    public UpdateCommand LocalizedUpdateCommand => _localizedUpdateCommand;

    public DataSourceId TargetDataSourceId => _targetDataSourceId;
}