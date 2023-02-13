using Janus.Commons.DataModels;
using Janus.Components.Translation;

namespace Janus.Wrapper.Translation;
public interface ILocalDataTranslator<TLocalData> : IDataTranslator<TLocalData, TabularData>
{
}
