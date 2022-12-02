using Janus.Commons.CommandModels;
using Janus.Commons.SchemaModels;

namespace Janus.Mediation.CommandMediationModels;

public sealed class DeleteCommandMediation
{
    private readonly DeleteCommand _localizedDeleteCommand;
    private readonly DataSourceId _targetDataSourceId;

    public DeleteCommandMediation(DeleteCommand localizedDeleteCommand, DataSourceId targetDataSourceId)
    {
        _localizedDeleteCommand = localizedDeleteCommand;
        _targetDataSourceId = targetDataSourceId;
    }

    public DeleteCommand LocalizedDeleteCommand => _localizedDeleteCommand;

    public DataSourceId TargetDataSourceId => _targetDataSourceId;
}