using FunctionalExtensions.Base;
using Janus.Mask.MaskedDataModel;

namespace Janus.Mask.WebApi.MaskedDataModel;

/// <summary>
/// Describes a local data for the web api mask - a collection of DTOs. 
/// Here the generic remained to accomodate the generic controller template. The alternative is to keep DTOs in object refs, and try to unbox them with a generic type at runtime.
/// </summary>
/// <typeparam name="TDto"></typeparam>
public class WebApiDtoData : MaskedData<object>
{
    private readonly IEnumerable<object> _data;
    private readonly Type _originalType;

    public WebApiDtoData(IEnumerable<object> data, Type? originalType = null)
    {
        _data = data ?? Enumerable.Empty<object>();
        _originalType = originalType ?? _data.FirstOrDefault()?.GetType() ?? typeof(object);
    }

    public override IReadOnlyList<object> Data => _data.ToList();

    public override long ItemCount => _data?.LongCount() ?? 0L;

    public IEnumerable<object> GetDataAs(Type type)
        => _data.Map(d => Convert.ChangeType(d, type));

    public IEnumerable<T> GetDataAs<T>()
        => _data.Map(d => (T)d);
}
