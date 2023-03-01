using Janus.Commons.DataModels;
using Janus.Components.Translation;
using Janus.Mask.MaskedDataModel;

namespace Janus.Wrapper.Translation;
public interface IMaskDataTranslator<TMaskedData, TMaskedItem> 
    : IDataTranslator<TMaskedData, TabularData>
    where TMaskedData : MaskedData<TMaskedItem>
{
}
