using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Components.Translation;
public interface ISchemaTranslator<TSchemaSource, TSchemaDestination>
{
    TSchemaDestination Translate(TSchemaSource source);
}
