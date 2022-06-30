using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Components.Translation;
public interface IDataTranslator<TSource, TDestination>
{
    TDestination Translate(TSource source);
}
