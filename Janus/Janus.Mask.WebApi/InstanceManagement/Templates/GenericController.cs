using Janus.Commons.SchemaModels;
using Janus.Logging;
using Janus.Mask.WebApi.InstanceManagement.Providers;
using Microsoft.AspNetCore.Mvc;

namespace Janus.Mask.WebApi.InstanceManagement.Templates;

[ApiController]
public abstract class GenericController<TId, TGetModel>
    : ControllerBase
    where TGetModel : BaseDto
{
    protected readonly ILogger<GenericController<TId, TGetModel>>? _logger;
    protected readonly QueryProvider<TId, TGetModel> _queryProvider;
    protected readonly CommandProvider<TId> _commandProvider;

    protected abstract TableauId TargetingTableauId { get; }
    protected abstract AttributeId IdentityAttributeId { get; }

    public int DEFAULT_ERROR_CODE => 500;

    protected GenericController(ProviderFactory providerResolver, ILogger? logger = null)
    {
        _logger = logger?.ResolveLogger<GenericController<TId, TGetModel>>();
        _queryProvider = providerResolver.ResolveQueryProvider<TId, TGetModel>(TargetingTableauId, IdentityAttributeId);
        _commandProvider = providerResolver.ResolveCommandProvider<TId>(TargetingTableauId, IdentityAttributeId);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<TGetModel>> Get(TId id)
    {
        _logger?.Debug($"Controller {ControllerContext.ActionDescriptor.ControllerName} action {ControllerContext.ActionDescriptor.ActionName} received id {id}");

        var result = _queryProvider.Get(id);

        return result ? Ok(result.Data) : Problem(statusCode: DEFAULT_ERROR_CODE, detail: result.Message);
    }

    [HttpGet]
    [Route("")]
    public async Task<ActionResult<IEnumerable<TGetModel>>> GetAll([FromQuery] string? selection)
    {
        _logger?.Debug($"Controller {ControllerContext.ActionDescriptor.ControllerName} action {ControllerContext.ActionDescriptor.ActionName}");

        var result = _queryProvider.GetAll(selection);

        return result ? Ok(result.Data) : Problem(statusCode: DEFAULT_ERROR_CODE, detail: result.Message);
    }

    //[HttpDelete]
    //[Route("{id}")]
    //public async Task<ActionResult> Delete(TId id)
    //{
    //    _logger?.Debug($"Controller {ControllerContext.ActionDescriptor.ControllerName} action {ControllerContext.ActionDescriptor.ActionName} received id {id}");
    //    var result = _commandProvider.Delete(id);
    //    return result ? Ok() : Problem(statusCode: DEFAULT_ERROR_CODE, detail: result.Message);
    //}

}
