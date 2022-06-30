using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.CsvFiles.Querying;
public class Projection
{
    private readonly List<string> _columnPaths;

    public IReadOnlyList<string> ColumnPaths => _columnPaths;

    public Projection(List<string> columnPaths!!)
    {
        _columnPaths = columnPaths;
    }
}
