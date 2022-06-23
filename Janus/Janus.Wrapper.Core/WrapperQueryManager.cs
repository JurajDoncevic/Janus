using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.Core;
public class WrapperQueryManager : IComponentQueryManager
{
    public Task<Result<TabularData>> RunQuery(Query query)
    {
        throw new NotImplementedException();
    }
}
