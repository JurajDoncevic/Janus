using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.CommandModels;

public class Mutation
{
    private readonly Dictionary<string, object?> _valueUpdates;

    internal Mutation(Dictionary<string, object?> valueUpdates!!)
    {
        _valueUpdates = valueUpdates;
    }

    public IReadOnlyDictionary<string, object?> ValueUpdates => _valueUpdates;

    public override bool Equals(object? obj)
    {
        return obj is Mutation mutation &&
               _valueUpdates.SequenceEqual(mutation._valueUpdates);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_valueUpdates);
    }
}
