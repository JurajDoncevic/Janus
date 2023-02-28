using Janus.Mask.MaskedDataModel;

namespace Janus.Mask.WebApi.MaskedDataModel;

/// <summary>
/// Describes a local data for the web api mask - a collection of DTOs. 
/// Here the generic remained to accomodate the generic controller template. The alternative is to keep DTOs in object refs, and try to unbox them with a generic type at runtime.
/// </summary>
/// <typeparam name="TDto"></typeparam>
public class WebApiDtoData<TDto> : MaskedData<TDto>
{
    private readonly IEnumerable<TDto> _data;

    public WebApiDtoData(IEnumerable<TDto> data)
    {
        _data = data ?? Enumerable.Empty<TDto>();
    }

    public override IReadOnlyList<TDto> Data => _data.ToList();

    public override long ItemCount => _data?.LongCount() ?? 0L;
}
