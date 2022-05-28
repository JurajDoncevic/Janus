using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.CommandModels;

public class Mutation
{
    private readonly Dictionary<string, object> _valueUpdates;

    internal Mutation(Dictionary<string, object> valueUpdates!!)
    {
        _valueUpdates = valueUpdates;
    }

    public IReadOnlyDictionary<string, object> ValueUpdates => _valueUpdates;
}
