using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.Core.SchemaInferrence.Model;
public class TableauInfo
{
    private readonly string _name;

    public TableauInfo(string name)
    {
        _name = name;
    }

    public string Name => _name;
}
