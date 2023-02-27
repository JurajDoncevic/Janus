﻿using Janus.Commons.DataModels;
using Janus.Components.Translation;
using Janus.Mask.LocalDataModel;

namespace Janus.Wrapper.Translation;
public interface ILocalDataTranslator<TLocalDataItem> : IDataTranslator<LocalData<TLocalDataItem>, TabularData>
{
}
