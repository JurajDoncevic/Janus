using Janus.Commons.SchemaModels;
using Janus.Mask.MaskedCommandModel;

namespace Janus.Mask.WebApi.MaskedCommandModel;
public sealed class WebApiDelete : MaskedDelete<string?>
{
    public WebApiDelete(TableauId target, string? selection) : base(target, selection)
    {
    }
}
