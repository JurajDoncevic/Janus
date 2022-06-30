using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.CsvFiles.Querying;
public class Selection
{
    private readonly HashSet<string> _inputColumnPaths;
    private readonly Func<Dictionary<string, object>, bool> _expression;

    public Selection(HashSet<string> inputColumnPaths, Func<Dictionary<string, object>, bool> expression)
    {
        _inputColumnPaths = inputColumnPaths;
        _expression = expression;
    }

    public IReadOnlySet<string> InputAttributes => _inputColumnPaths;

    public Func<Dictionary<string, object>, bool> Expression => _expression;
}
