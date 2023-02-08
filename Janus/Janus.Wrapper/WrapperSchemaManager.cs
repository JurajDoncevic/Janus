using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.SchemaModels;
using Janus.Components;
using Janus.Logging;
using Janus.Wrapper.SchemaInferrence;

namespace Janus.Wrapper;
/// <summary>
/// Schema manager for a wrapper component
/// </summary>
public abstract class WrapperSchemaManager : IComponentSchemaManager
{
    private readonly SchemaInferrer _schemaInferrer;
    private Option<DataSource> _currentSchema;

    private readonly ILogger<WrapperSchemaManager>? _logger;

    public WrapperSchemaManager(SchemaInferrer schemaInferrer, ILogger? logger = null)
    {
        _schemaInferrer = schemaInferrer;
        _logger = logger?.ResolveLogger<WrapperSchemaManager>();
    }

    public Option<DataSource> GetCurrentOutputSchema()
        => _currentSchema;

    public async Task<Result<DataSource>> ReloadOutputSchema()
        => (await Task.FromResult(
            _schemaInferrer.InferSchemaModel()
                .Pass(result => _currentSchema = Option<DataSource>.Some(result.Data))))
                .Pass(r => _logger?.Info($"Reloaded schema with name {r.Data.Name} and version {r.Data.Version}."),
                      r => _logger?.Info($"Failed to load schema with message: {r.Message}"));
}
