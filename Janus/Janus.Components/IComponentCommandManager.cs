using Janus.Commons.CommandModels;
using Janus.Communication.Remotes;

namespace Janus.Components;

public interface IExecutingCommandManager
{
    /// <summary>
    /// Executes a command on the component's schema
    /// </summary>
    /// <param name="command">Command to execute</param>
    /// <returns></returns>
    public Task<Result> RunCommand(BaseCommand command);
}

public interface IDelegatingCommandManager
{
    /// <summary>
    /// Executes a command on a remote component
    /// </summary>
    /// <param name="command"></param>
    /// <param name="remotePoint"></param>
    /// <returns></returns>
    public Task<Result> RunCommandOnComponent(BaseCommand command, RemotePoint remotePoint);
}