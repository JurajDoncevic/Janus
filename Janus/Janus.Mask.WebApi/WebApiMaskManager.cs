using FunctionalExtensions.Base.Resulting;
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
