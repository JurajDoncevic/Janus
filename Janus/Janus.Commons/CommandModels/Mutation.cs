namespace Janus.Commons.CommandModels;

/// <summary>
/// Describes a mutation clause
/// </summary>
public class Mutation
{
    private readonly Dictionary<string, object?> _valueUpdates;

    /// <summary>
    /// Value updates specification
    /// </summary>
    public IReadOnlyDictionary<string, object?> ValueUpdates => _valueUpdates;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="valueUpdates">Value updates specification. <b>Use attribute names, not ids!</b></param>
    internal Mutation(Dictionary<string, object?> valueUpdates!!)
    {
        _valueUpdates = valueUpdates;
    }

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
