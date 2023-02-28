using Janus.Mask.LocalDataModel;

namespace Janus.Mask.WebApi.LocalDataModel;

/// <summary>
/// Describes a local data for the web api mask - a collection of DTOs
/// </summary>
/// <typeparam name="TDto"></typeparam>
public class WebApiDtoData<TDto> : LocalData<TDto>
{
    private readonly IEnumerable<TDto> _data;

    public WebApiDtoData(IEnumerable<TDto> data)
    {
        _data = data ?? Enumerable.Empty<TDto>();
    }

    public override IReadOnlyList<TDto> Data => _data.ToList();

    public override long ItemCount => _data?.LongCount() ?? 0L;
}
