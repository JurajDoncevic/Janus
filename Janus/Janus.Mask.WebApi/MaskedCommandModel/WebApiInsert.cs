using Janus.Commons.SchemaModels;
using Janus.Mask.MaskedCommandModel;

namespace Janus.Mask.WebApi.MaskedCommandModel;
public sealed class WebApiInsert : MaskedInsert<object>
{
    public WebApiInsert(object instantiation, TableauId target) : base(instantiation, target)
    {
    }
}
