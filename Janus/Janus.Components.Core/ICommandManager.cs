using Janus.Commons.CommandModels;

namespace Janus.Components.Core;

public interface ICommandManager
{
    public Result ExecuteCommandOnNode(BaseCommand command, string nodeId);
    public Result ExecuteCommandGlobally(BaseCommand command);
}
