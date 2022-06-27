using Janus.Commons.SchemaModels;
using Janus.Components;
using Janus.Wrapper.Core.SchemaInferrence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.Core;
public class WrapperSchemaManager : IComponentSchemaManager
{
    private readonly SchemaInferrer _schemaInferrer;
    private readonly DataSource? _currentSchema;

    public WrapperSchemaManager(SchemaInferrer schemaInferrer)
    {
        _schemaInferrer = schemaInferrer;
    }

    public async Task<Result<DataSource>> GetCurrentSchema()
        => await Task.FromResult(_currentSchema != null ? Result<DataSource>.OnSuccess(_currentSchema) : Result<DataSource>.OnFailure<DataSource>("No schema loaded"));

    public async Task<Result<DataSource>> ReloadSchema(object? transformations = null)
        => await Task.Run(() => _schemaInferrer.InferSchemaModel());
}
