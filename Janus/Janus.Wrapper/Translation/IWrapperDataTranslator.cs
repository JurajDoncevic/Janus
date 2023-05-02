using Janus.Base.Resulting;
using Janus.Commons.DataModels;
using Janus.Components.Translation;

namespace Janus.Wrapper.Translation;
public interface IWrapperDataTranslator<TLocalData> : IDataTranslator<TLocalData, TabularData>
{
}
