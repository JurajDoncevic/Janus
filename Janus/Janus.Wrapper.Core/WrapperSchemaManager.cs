using Janus.Commons.SchemaModels;
using Janus.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.Core;
public class WrapperSchemaManager : IComponentSchemaManager
{
    public Task<Result<DataSource>> GetCurrentSchema()
    {
        throw new NotImplementedException();
    }

    public Task<Result<DataSource>> ReloadSchema(object? transformations = null)
    {
        throw new NotImplementedException();
    }
}
