using Janus.Mask.WebApi.InstanceManagement.Templates;
using Janus.Mask.WebApi.InstanceManagement.Typing;
using Janus.Logging;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;
using Janus.Mask.WebApi.InstanceManagement.Providers;

namespace Janus.Mask.WebApi.InstanceManagement;
public class GenericControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
{
    private readonly IEnumerable<ControllerTyping> _controllerTypings;
    private readonly TypeFactory _typeFactory;

    public GenericControllerFeatureProvider(IEnumerable<ControllerTyping> controllerTypings, TypeFactory typeFactory)
    {
        _controllerTypings = controllerTypings;
        _typeFactory = typeFactory;
    }

    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        // This is designed to run after the default ControllerTypeProvider, 
        // so the list of 'real' controllers has already been populated.
        foreach (var controllerTyping in _controllerTypings)
        {
            var typeName = CapitalizeName(controllerTyping.ControllerName) + "Controller";
            if (!feature.Controllers.Any(t => t.Name == typeName))
            {
                // There's no 'real' controller for this entity, so add the generic version.
                var controllerType =
                    (typeof(GenericController<,>))
                    .MakeGenericType(controllerTyping.IdPropertyType,
                                     controllerTyping.GetDto.GenerateType(_typeFactory))
                    .GetTypeInfo();

                var ctrType =
                    _typeFactory.CreateControllerType(controllerTyping, "JanusGenericMask.InstanceManagement.Web.Dynamic", controllerType, new Type[] { typeof(ProviderFactory), typeof(ILogger) })
                    .GetTypeInfo();

                feature.Controllers.Add(ctrType);
            }
        }
    }

    private string CapitalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return name;
        }
        var capitalChar = char.ToUpper(name.Trim().FirstOrDefault());

        return capitalChar + name[1..];
    }
}
