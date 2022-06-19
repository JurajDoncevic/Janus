using FunctionalExtensions.Base.Results;
using Janus.Commons.CommandModels;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Communication.Remotes;

namespace Janus.Components.Core;

public interface IComponentController
{
    /// <summary>
    /// Initiates a register operation for a node
    /// </summary>
    /// <param name="address">Target node address</param>
    /// <param name="port">Target node port</param>
    /// <returns></returns>
    public Task<Result<RemotePoint>> RegisterRemotePoint(string address, int port);

    /// <summary>
    /// Initiates an operation to unregister a node
    /// </summary>
    /// <param name="remotePoint">Target node's remote point</param>
    /// <returns></returns>
    public Task<Result> UnregisterRemotePoint(RemotePoint remotePoint);

    /// <summary>
    /// Gets all the currently registered remote points
    /// </summary>
    /// <returns></returns>
    public IEnumerable<RemotePoint> GetRegisteredRemotePoints();

    /// <summary>
    /// Sends a HELLO ping to a remote point. Doesn't initiate a register operation
    /// </summary>
    /// <param name="remotePoint">Target node's remote point</param>
    /// <returns></returns>
    public Task<Result<RemotePoint>> SendHello(RemotePoint remotePoint);

    /// <summary>
    /// Sends a HELLO ping to a remote point. Doesn't initiate a register operation
    /// </summary>
    /// <param name="address">Target node address</param>
    /// <param name="port">Target node port</param>
    /// <returns></returns>
    public Task<Result<RemotePoint>> SendHello(string address, int port);

    /// <summary>
    /// Runs a command on this component
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public Task<Result> RunCommand(BaseCommand command);

    /// <summary>
    /// Runs a query on this component
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public Task<Result<TabularData>> RunQuery(Query query);

    /// <summary>
    /// Gets the schema on this component
    /// </summary>
    /// <returns></returns>
    public Task<Result<DataSource>> GetSchema();

    /// <summary>
    /// Save currently registered remote points to file (json)
    /// </summary>
    /// <param name="filePath">Output file path</param>
    /// <returns></returns>
    public Task<Result> SaveRegisteredRemotePoints(string filePath);

}
