using FunctionalExtensions.Base.Results;
using Janus.Commons.CommandModels;

namespace Janus.Components;

public interface IComponentCommandManager
{
    public Task<Result> ExecuteCommandOnNode(BaseCommand command, string nodeId);
    public Task<Result> ExecuteCommandGlobally(BaseCommand command);
}
