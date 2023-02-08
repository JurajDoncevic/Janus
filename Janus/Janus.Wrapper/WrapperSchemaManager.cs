using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.SchemaModels;
using Janus.Components;
using Janus.Wrapper.SchemaInferrence;

namespace Janus.Wrapper;
/// <summary>
/// Schema manager for a wrapper component
/// </summary>
public abstract class WrapperSchemaManager : IComponentSchemaManager
{
    private readonly SchemaInferrer _schemaInferrer;
    private Option<DataSource> _currentSchema;
    public WrapperSchemaManager(SchemaInferrer schemaInferrer)
    {
        _schemaInferrer = schemaInferrer;
    }

    public Option<DataSource> GetCurrentOutputSchema()
        => _currentSchema;

    public Task<Result<DataSource>> ReloadOutputSchema()
        => Task.FromResult(
            _schemaInferrer.InferSchemaModel()
                .Pass(result => _currentSchema = Option<DataSource>.Some(result.Data)));
}
