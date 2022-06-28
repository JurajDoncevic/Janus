using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.CsvFiles.Querying;
public class Selection
{
    private readonly HashSet<string> _inputAttributes;
    private readonly Func<HashSet<string>, bool> _expression;

    public Selection(HashSet<string> inputAttributes, Func<HashSet<string>, bool> expression)
    {
        _inputAttributes = inputAttributes;
        _expression = expression;
    }
}
