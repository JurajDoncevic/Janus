using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Results;
using Janus.Commons.CommandModels;
using Janus.Wrapper.Sqlite.LocalCommanding;
using Janus.Wrapper.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.Sqlite.Translation;
public class SqliteCommandTranslator
    : ILocalCommandTranslator<SqliteDelete, SqliteInsert, SqliteUpdate, string, string, string>
{
    public Result<SqliteDelete> TranslateDelete(DeleteCommand query)
    {
        throw new NotImplementedException();
    }

    public Result<SqliteInsert> TranslateInsert(InsertCommand query)
    {
        throw new NotImplementedException();
    }

    public Result<string> TranslateMutation(Option<Mutation> mutation)
    {
        throw new NotImplementedException();
    }

    public Result<string> TranslateProjection(Option<Instantiation> instantiation)
    {
        throw new NotImplementedException();
    }

    public Result<string> TranslateSelection(Option<CommandSelection> selection)
    {
        throw new NotImplementedException();
    }

    public Result<SqliteUpdate> TranslateUpdate(UpdateCommand query)
    {
        throw new NotImplementedException();
    }
}
