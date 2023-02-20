using Janus.Commons.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Lenses;
public class TabularDataObjectsLens : Lens<TabularData, IEnumerable<object>>
{
    internal TabularDataObjectsLens() : base()
    {
    }

    public override Func<IEnumerable<object>, TabularData, TabularData> Put => throw new NotImplementedException();

    public override Func<TabularData, IEnumerable<object>> Get => throw new NotImplementedException();
}
