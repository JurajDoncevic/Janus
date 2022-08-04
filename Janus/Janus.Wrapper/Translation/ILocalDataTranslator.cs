using FunctionalExtensions.Base.Results;
using Janus.Commons.DataModels;

namespace Janus.Wrapper.Translation;
public interface ILocalDataTranslator<TLocalData>
{
    public Result<TabularData> TranslateToTabularData(TLocalData data);
}
