using Janus.Commons.SchemaModels;
using Janus.Mask.LocalCommanding;

namespace Janus.Mask.WebApi.LocalCommanding;
public sealed class WebApiInsert : LocalInsert<object>
{
    public WebApiInsert(object instantiation, TableauId target) : base(instantiation, target)
    {
    }
}
