using Janus.Commons.SchemaModels;
using Janus.Mask.MaskedCommandModel;

namespace Janus.Mask.WebApi.MaskedCommandModel;
public sealed class WebApiUpdate : MaskedUpdate<string?, object>
{
    public WebApiUpdate(TableauId target, object mutation, string? selection = null) : base(target, selection, mutation)
    {
    }
}
