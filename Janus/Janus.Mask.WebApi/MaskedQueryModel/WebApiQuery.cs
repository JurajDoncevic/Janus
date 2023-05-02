using Janus.Base;
using Janus.Commons.SchemaModels;
using Janus.Mask.MaskedQueryModel;

namespace Janus.Mask.WebApi.MaskedQueryModel;
public sealed class WebApiQuery : MaskedQuery<TableauId, string?, Unit, Unit>
{
    private readonly Type _expectingReturnDtoType;

    public WebApiQuery(TableauId startingWith, string? selection = null, Type? expectingReturnDtoType = null) : base(startingWith, selection, UnitExtensions.Unit(), UnitExtensions.Unit())
    {
        _expectingReturnDtoType = expectingReturnDtoType ?? typeof(object);
    }

    public Type ExpectingReturnDtoType => _expectingReturnDtoType;
}
