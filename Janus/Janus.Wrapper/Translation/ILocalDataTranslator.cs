using FunctionalExtensions.Base.Resulting;
using Janus.Commons.DataModels;

namespace Janus.Wrapper.Translation;
public interface ILocalDataTranslator<TLocalData>
{
    public Result<TabularData> TranslateToTabularData(TLocalData data);
}
