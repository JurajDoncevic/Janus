using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.CommandModels;

public class InsertCommand
{
    private readonly string _onTableauId;
    private readonly Instantiation _instantiation;

    public InsertCommand(string onTableauId!!, Instantiation instantiation!!)
    {
        _onTableauId = onTableauId;
        _instantiation = instantiation;
    }

    public string OnTableauId => _onTableauId;

    public Instantiation Instantiation => _instantiation;
}
