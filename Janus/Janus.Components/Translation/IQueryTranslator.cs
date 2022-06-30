using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Components.Translation;
public interface IQueryTranslator<TQuerySource, TSelectionSource, TJoiningSource, TProjectionSource, TQueryDestination,  TSelectionDestination, TJoiningDestination,  TProjectionDestination>
{
    TQueryDestination Translate(TQuerySource query);
    TSelectionDestination TranslateSelection(Option<TSelectionSource> selection);
    TJoiningDestination TranslateJoin(Option<TJoiningSource> joining);
    TProjectionDestination TranslateProjection(Option<TProjectionSource> projection);
}
