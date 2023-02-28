using FunctionalExtensions.Base;
using Janus.Commons.SchemaModels;
using Janus.Mask.MaskedQueryModel;

namespace Janus.Mask.WebApi.MaskedQueryModel;
public sealed class WebApiQuery : MaskedQuery<TableauId, string?, Unit, Unit>
{
    public WebApiQuery(TableauId startingWith, string? selection = null) : base(startingWith, selection, UnitExtensions.Unit(), UnitExtensions.Unit())
    {
    }
}
