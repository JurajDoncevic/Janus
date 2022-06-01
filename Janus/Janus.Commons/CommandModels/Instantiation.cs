using Janus.Commons.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.CommandModels;

public class Instantiation
{
    private readonly TabularData _tableauData;

    internal Instantiation(TabularData tableauData!!)
    {
        _tableauData = tableauData;
    }

    public TabularData TableauData => _tableauData;

    public override bool Equals(object? obj)
    {
        return obj is Instantiation instantiation &&
               _tableauData.Equals(instantiation._tableauData);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_tableauData);
    }
}
