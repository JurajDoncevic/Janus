using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.Core.SchemaInferrence.Model;
public class DataSourceInfo
{
    private readonly string _name;

    public DataSourceInfo(string name)
    {
        _name = name;
    }

    public string Name => _name;
}
