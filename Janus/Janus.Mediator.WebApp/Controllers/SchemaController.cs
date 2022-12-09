using Janus.Communication.Remotes;
using Microsoft.AspNetCore.Mvc;

namespace Janus.Mediator.WebApp.Controllers;
public class SchemaController : Controller
{
    private readonly MediatorManager _mediatorManager;

    public SchemaController(MediatorManager mediatorManager)
    {
        _mediatorManager = mediatorManager;
    }

    public IActionResult Index()
    {
        // gets the current mediated schema
        return View();
    }

    public IActionResult VisibleSchemas()
    {
        // gets all schemas and remote points from components
        return View();
    }

    public IActionResult LoadedSchemas()
    {
        // gets all schemas loaded schemas available for mediation
        return View();
    }

    public IActionResult LoadSchemas(IEnumerable<RemotePoint> remotePoints)
    {
        // load all schemas from selected remote points
        return View();
    }

    public IActionResult CurrentMediation()
    {
        // gets the current mediation script, make a GET and POST
        return View();
    }

    public IActionResult CurrentMediation(string mediationScript)
    {
        // sets the current mediation of loaded schemas with a mediation script
        return View();
    }
}
