using Janus.Mask.WebApi.InstanceManagement.Typing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Mask.WebApi.LocalSchemaModel;

/// <summary>
/// Describes a Web API masked schema as a collection of controller typings
/// </summary>
public sealed class WebApiTyping
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
