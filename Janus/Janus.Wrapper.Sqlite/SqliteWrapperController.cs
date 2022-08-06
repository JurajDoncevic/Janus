using Janus.Communication.Nodes.Implementations;
using Janus.Wrapper.Sqlite.LocalCommanding;
using Janus.Wrapper.Sqlite.LocalDataModel;
using Janus.Wrapper.Sqlite.LocalQuerying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.Sqlite;
public class SqliteWrapperController
    : WrapperController<SqliteQuery, SqliteDelete, SqliteInsert, SqliteUpdate, string, string, string, SqliteTabularData, string, string>
{
    public SqliteWrapperController(WrapperCommunicationNode communicationNode, SqliteWrapperQueryManager queryManager, SqliteWrapperCommandManager commandManager, SqliteWrapperSchemaManager schemaManager) : base(communicationNode, queryManager, commandManager, schemaManager)
    {
    }
}
