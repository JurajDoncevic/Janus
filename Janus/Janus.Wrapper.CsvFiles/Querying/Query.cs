using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.CsvFiles.Querying;
public class Query
{
    private readonly Projection _projection;
    private readonly Selection _selection;
    private readonly Joining _joining;
    private readonly string _onFilePath;

    public Query(string onFilePath, Projection projection, Selection selection, Joining joining)
    {
        _projection = projection;
        _selection = selection;
        _joining = joining;
        _onFilePath = onFilePath;
    }

    public Projection Projection => _projection;

    public Selection Selection => _selection;

    public Joining Joining => _joining;

    public string OnFilePath => _onFilePath;
}
