using Janus.Commons.SchemaModels;
using Janus.Mask.LocalCommanding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Mask.WebApi.LocalCommanding;
public sealed class WebApiUpdate : LocalUpdate<string?, object>
{
    public WebApiUpdate(TableauId target, object mutation, string? selection = null) : base(target, selection, mutation)
    {
    }
}
