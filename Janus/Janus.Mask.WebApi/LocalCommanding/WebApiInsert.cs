using Janus.Commons.SchemaModels;
using Janus.Mask.LocalCommanding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Mask.WebApi.LocalCommanding;
public sealed class WebApiInsert : LocalInsert<object>
{
    public WebApiInsert(object instantiation, TableauId target) : base(instantiation, target)
    {
    }
}
