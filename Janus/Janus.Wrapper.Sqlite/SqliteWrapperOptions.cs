using Janus.Commons;
using Janus.Communication.Remotes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.Sqlite;
public class SqliteWrapperOptions : WrapperOptions
{
    public SqliteWrapperOptions(
        string nodeId,
        int listenPort,
        int timeoutMs,
        CommunicationFormats communicationFormat,
        NetworkAdapterTypes networkAdapterType,
        bool eagerStartup,
        IEnumerable<UndeterminedRemotePoint> startupRemotePoints,
        bool startupInferSchema,
        string sourceConnectionString,
        bool allowsCommands,
        string persistenceConnectionString,
        string dataSourceName) 
        : base(nodeId,
               listenPort,
               timeoutMs,
               communicationFormat,
               networkAdapterType,
               eagerStartup,
               startupRemotePoints,
               startupInferSchema,
               sourceConnectionString,
               allowsCommands,
               persistenceConnectionString,
               dataSourceName)
    {
    }
}
