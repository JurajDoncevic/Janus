using Janus.Commons.QueryModels;
using Janus.Components.Translation;
using Janus.Wrapper.Core;
using Janus.Wrapper.Core.Querying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.CsvFiles;
public class CsvFilesWrapperQueryManager : WrapperQueryManager<Query, Selection, Joining, Projection>
{
    public CsvFilesWrapperQueryManager(IWrapperQueryRunner<Query> queryRunner, IQueryTranslator<Query, Selection, Joining, Projection, Query, Selection, Joining, Projection> queryTranslator) : base(queryRunner, queryTranslator)
    {
    }
}
