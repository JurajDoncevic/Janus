using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Results;
using Janus.Commons.SchemaModels;
using Janus.Components;
using Janus.Wrapper.SchemaInferrence;

namespace Janus.Wrapper;
public sealed class WrapperSchemaManager : IComponentSchemaManager
{
    private readonly SchemaInferrer _schemaInferrer;
    private DataSource? _currentSchema;
    public WrapperSchemaManager(SchemaInferrer schemaInferrer)
    {
        _schemaInferrer = schemaInferrer;
    }

    public Task<Result<DataSource>> GetCurrentSchema()
        => Task.FromResult(
            _currentSchema != null
                ? Result<DataSource>.OnSuccess(_currentSchema)
                : Result<DataSource>.OnFailure("No schema loaded currently"));

    public Task<Result<DataSource>> ReloadSchema()
        => Task.FromResult(
            _schemaInferrer.InferSchemaModel()
                .Pass(result => _currentSchema = result.Data));
            
}
