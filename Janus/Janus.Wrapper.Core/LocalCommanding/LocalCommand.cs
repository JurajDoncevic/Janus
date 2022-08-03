using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.Core.LocalCommanding;
public abstract class LocalCommand
{
    private readonly string _target;

    protected LocalCommand(string target)
    {
        _target = target;
    }

    public string Target => _target;
}
