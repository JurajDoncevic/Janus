
namespace Janus.Commons.QueryModels;

public class Selection
{
    private readonly string _expression;
    public string Expression => _expression;
    public Selection(string expression)
    {
        _expression = expression;
    }
}
