using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Results;
using Janus.Commons.SchemaModels;
using Janus.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.Core;
public sealed class WrapperSchemaManager : IComponentSchemaManager
{
    public Task<Result<DataSource>> GetCurrentSchema()
    {
        // get the currently inferred schema
        throw new NotImplementedException();
    }

    public Task<Result<DataSource>> ReloadSchema()
    {
        // inferr the schema and set it as current
        throw new NotImplementedException();
    }
}
