using Janus.Wrapper.Sqlite.LocalDataModel;
using Janus.Wrapper.Sqlite.LocalQuerying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.Sqlite;
public class SqliteWrapperController 
    : WrapperController<string, string, string, SqliteTabularData, string, string, SqliteQuery>
{
}
