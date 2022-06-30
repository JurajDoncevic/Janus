using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.CsvFiles.Querying;
public class Joining
{
    private readonly List<Join> _joins;

    public IReadOnlyList<Join> Joins => _joins;

    public Joining(List<Join> joins!!)
    {
        _joins = joins;
    }
}
