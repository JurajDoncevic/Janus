using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Mask.WebApi.InstanceManagement.Templates;
public interface IPutController<TPutModel>
{
    [HttpPut]
    public ActionResult Update([FromQuery] string selection, [FromBody] TPutModel model);
}
