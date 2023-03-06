using FunctionalExtensions.Base.Resulting;
using Janus.Commons.DataModels;
using Janus.Lenses;
using Janus.Mask.WebApi.Lenses;
using Janus.Mask.WebApi.MaskedDataModel;
using Janus.Wrapper.Translation;

namespace Janus.Mask.WebApi.Translation;
public class WebApiDataTranslator<TDto> : IMaskDataTranslator<WebApiDtoData<TDto>, TDto>
{
    private readonly TabularDataDtoLens<TDto> _dataLens;
    private readonly string _columnNamePrefix;

    public WebApiDataTranslator(string? columnNamePrefix = null)
    {
        _dataLens= TabularDataDtoLens.Construct<TDto>(columnNamePrefix);
        _columnNamePrefix = columnNamePrefix ?? string.Empty;
    }

    public Result<TabularData> Translate(WebApiDtoData<TDto> source)
        => Results.AsResult(() =>
        {
            return _dataLens.Put(source.Data, null);
        });

    public Result<WebApiDtoData<TDto>> Translate(TabularData destination)
        => Results.AsResult(() =>
        {
            return new WebApiDtoData<TDto>(_dataLens.Get(destination));
        });
}
