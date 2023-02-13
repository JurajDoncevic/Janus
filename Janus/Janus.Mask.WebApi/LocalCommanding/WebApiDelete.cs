using Janus.Commons.SchemaModels;
using Janus.Mask.LocalCommanding;

namespace Janus.Mask.WebApi.LocalCommanding;
public sealed class WebApiDelete : LocalDelete<string?>
{
    public WebApiDelete(TableauId target, string? selection) : base(target, selection)
    {
    }
}
