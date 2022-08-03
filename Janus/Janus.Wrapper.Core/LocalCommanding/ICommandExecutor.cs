using FunctionalExtensions.Base.Results;

namespace Janus.Wrapper.Core.LocalCommanding;

public interface ICommandExecutor<TSelection, TMutation, TInstantiation>
{
    public Task<Result> ExecuteCommand(LocalCommand localQuery);
}