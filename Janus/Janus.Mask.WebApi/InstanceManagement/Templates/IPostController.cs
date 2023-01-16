using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Mask.WebApi.InstanceManagement.Templates;
public interface IPostController<TPostModel>
{
    [HttpPost]
    [Route("")]
    public ActionResult Create([FromBody] TPostModel model);
}
