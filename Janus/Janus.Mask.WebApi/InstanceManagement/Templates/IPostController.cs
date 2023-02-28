using Microsoft.AspNetCore.Mvc;

namespace Janus.Mask.WebApi.InstanceManagement.Templates;
public interface IPostController<TPostModel>
{
    [HttpPost]
    [Route("")]
    public ActionResult Create([FromBody] TPostModel model);
}
