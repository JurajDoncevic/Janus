using Microsoft.AspNetCore.Mvc;

namespace Janus.Mask.WebApi.InstanceManagement.Templates;
public interface IPutController<TPutModel>
{
    [HttpPut]
    public ActionResult Update([FromQuery] string selection, [FromBody] TPutModel model);
}
