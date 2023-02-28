using Janus.Communication.Nodes.Implementations;
using Janus.Logging;
using Janus.Mask.WebApi.InstanceManagement.Typing;
using Janus.Mask.WebApi.Translation;

namespace Janus.Mask.WebApi;
public sealed class WebApiMaskSchemaManager : MaskSchemaManager<IEnumerable<ControllerTyping>>
{
    private readonly WebApiSchemaTranslator _schemaTranslator;

    public WebApiMaskSchemaManager(MaskCommunicationNode communicationNode, WebApiSchemaTranslator schemaTranslator, ILogger? logger = null) : base(communicationNode, schemaTranslator, logger)
    {
        _schemaTranslator = schemaTranslator;
    }
}
