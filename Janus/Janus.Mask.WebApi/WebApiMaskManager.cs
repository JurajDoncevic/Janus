using FunctionalExtensions.Base.Resulting;
using Janus.Communication.Nodes.Implementations;
using Janus.Logging;
using Janus.Mask.Persistence;
using Janus.Mask;
using JanusGenericMask.InstanceManagement.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Mask.WebApi;
public class WebApiMaskManager : MaskManager
{
    private readonly WebApiInstance _webApiInstance;

    public WebApiMaskManager(MaskCommunicationNode communicationNode, MaskQueryManager queryManager, MaskCommandManager commandManager, MaskSchemaManager schemaManager, MaskPersistenceProvider persistenceProvider, WebApiMaskOptions maskOptions, ILogger? logger = null) 
        : base(communicationNode, queryManager, commandManager, schemaManager, persistenceProvider, maskOptions, logger)
    {
        _webApiInstance = new WebApiInstance(maskOptions.WebApiOptions, logger);
    }

    public Result StartWebApi()
        => Results.AsResult(() =>
        {
            return _webApiInstance.StartApplication(GetCurrentSchema());
        });

    public Result StopWebApi()
        => Results.AsResult(() =>
        {
            return _webApiInstance.StopApplication();
        });
}
