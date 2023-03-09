using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Mask.WebApi.InstanceManagement.Templates;
public interface IDeleteController<TId>
{
    [HttpDelete]
    [Route("{id}")]
    public ActionResult Delete([FromRoute]TId id);
}
