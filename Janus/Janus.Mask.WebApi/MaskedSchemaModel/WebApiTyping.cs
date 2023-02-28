using Janus.Mask.MaskedSchemaModel;

namespace Janus.Mask.WebApi.MaskedSchemaModel;

/// <summary>
/// Describes a Web API masked schema as a collection of controller typings
/// </summary>
public sealed class WebApiTyping : MaskedDataSource
{
    private readonly IEnumerable<ControllerTyping> _controllerTypings;

    public WebApiTyping(IEnumerable<ControllerTyping> controllerTypings)
    {
        _controllerTypings = controllerTypings;
    }

    /// <summary>
    /// Controller typings in the masked schema
    /// </summary>
    public IReadOnlyList<ControllerTyping> ControllerTypings => _controllerTypings.ToList();
}
