using Janus.Commons.CommandModels;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Communication.Remotes;

namespace Janus.Components;

/// <summary>
/// Command manager that runs commands on its own schema 
/// </summary>
public interface IComponentCommandManager
{
    /// <summary>
    /// Runs a command on the component's schema
    /// </summary>
    /// <param name="command">Command to execute</param>
    /// <returns></returns>
    public Task<Result> RunCommand(BaseCommand command);
}

/// <summary>
/// Command manager that delegates command execution to other components
/// </summary>
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

/// <summary>
/// Command manager that works over a mediated schema
/// </summary>
/// <typeparam name="TSchemaManager">Type of schema manager - Mediating</typeparam>
public interface IMediatingCommandManager<TSchemaManager> where TSchemaManager : IMediatingSchemaManager
{
    /// <summary>
    /// Runs a command on a mediated schema in the schema manager
    /// </summary>
    /// <param name="command">Command over mediated schema</param>
    /// <param name="schemaManager">Current mediating schema manager</param>
    /// <returns></returns>
    public Task<Result> RunCommand(BaseCommand command, TSchemaManager schemaManager);
}