
namespace Janus.Commons.QueryModels;

/// <summary>
/// Describes a query selection clause
/// </summary>
public class Selection
{
    private readonly string _expression;
    public string Expression => _expression;
    public Selection(string expression)
    {
        _expression = expression;
    }

    public override bool Equals(object? obj)
    {
        return obj is Selection selection &&
               _expression == selection._expression;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_expression);
    }
}
