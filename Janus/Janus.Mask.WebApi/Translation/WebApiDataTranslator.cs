using FunctionalExtensions.Base.Resulting;
using Janus.Commons.DataModels;
using Janus.Lenses.Implementations;
using Janus.Mask.WebApi.MaskedDataModel;
using Janus.Wrapper.Translation;

namespace Janus.Mask.WebApi.Translation;
public class WebApiDataTranslator : IMaskDataTranslator<WebApiDtoData, object>
{
    private readonly TabularDataDtoLens<object> _dataLens;
    private readonly string _columnNamePrefix;

    public WebApiDataTranslator(string? columnNamePrefix = null, Type? originalType = null)
    {
        _dataLens= TabularDataDtoLenses.Construct<object>(columnNamePrefix, originalType);
        _columnNamePrefix = columnNamePrefix ?? string.Empty;
    }

    public Result<TabularData> Translate(WebApiDtoData source)
        => Results.AsResult(() =>
        {
            return _dataLens.Put(source.Data, null);
        });

    public Result<WebApiDtoData> Translate(TabularData destination)
        => Results.AsResult(() =>
        {
            return new WebApiDtoData(_dataLens.Get(destination));
        });
}
