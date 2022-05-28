using Janus.Commons.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.CommandModels;

public class Instantiation
{
    private readonly TableauData _tableauData;

    internal Instantiation(TableauData tableauData!!)
    {
        _tableauData = tableauData;
    }

    public TableauData TableauData => _tableauData;
}
