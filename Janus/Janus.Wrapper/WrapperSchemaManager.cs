using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.SchemaModels;
using Janus.Components;
using Janus.Wrapper.SchemaInferrence;

namespace Janus.Wrapper;
public class WrapperSchemaManager : IComponentSchemaManager
{
    private readonly SchemaInferrer _schemaInferrer;
    private DataSource? _currentSchema;
    public WrapperSchemaManager(SchemaInferrer schemaInferrer)
    {
        _schemaInferrer = schemaInferrer;
    }

    public Task<Result<DataSource>> GetCurrentOutputSchema()
        => Task.FromResult(
            _currentSchema != null
                ? Results.OnSuccess(_currentSchema)
                : Results.OnFailure<DataSource>("No schema loaded currently"));

    public Task<Result<DataSource>> ReloadOutputSchema()
        => Task.FromResult(
            _schemaInferrer.InferSchemaModel()
                .Pass(result => _currentSchema = result.Data));

}
