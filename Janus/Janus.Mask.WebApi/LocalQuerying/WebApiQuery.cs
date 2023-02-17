using FunctionalExtensions.Base;
using Janus.Commons.SchemaModels;
using Janus.Mask.LocalQuerying;

namespace Janus.Mask.WebApi.LocalQuerying;
public sealed class WebApiQuery : LocalQuery<TableauId, string?, Unit, Unit>
{
    public WebApiQuery(TableauId startingWith, string? selection = null) : base(startingWith, selection, UnitExtensions.Unit(), UnitExtensions.Unit())
    {
    }
}
