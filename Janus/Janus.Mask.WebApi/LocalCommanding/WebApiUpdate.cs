using Janus.Commons.SchemaModels;
using Janus.Mask.LocalCommanding;

namespace Janus.Mask.WebApi.LocalCommanding;
public sealed class WebApiUpdate : LocalUpdate<string?, object>
{
    public WebApiUpdate(TableauId target, object mutation, string? selection = null) : base(target, selection, mutation)
    {
    }
}
