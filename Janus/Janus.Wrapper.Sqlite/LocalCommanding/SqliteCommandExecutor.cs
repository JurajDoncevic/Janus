using FunctionalExtensions.Base.Results;
using Janus.Wrapper.LocalCommanding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.Sqlite.LocalCommanding;
public class SqliteCommandExecutor : ICommandExecutor<SqliteDelete, SqliteInsert, SqliteUpdate, string, string, string>
{
    public Task<Result> ExecuteDeleteCommand(SqliteDelete command)
    {
        throw new NotImplementedException();
    }

    public Task<Result> ExecuteInsertCommand(SqliteInsert command)
    {
        throw new NotImplementedException();
    }

    public Task<Result> ExecuteUpdateCommand(SqliteUpdate command)
    {
        throw new NotImplementedException();
    }
}
