using Janus.Commons.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.CommandModels;

public class Instantiation
{
    private readonly TabularData _tabularData;

    internal Instantiation(TabularData tabularData!!)
    {
        _tabularData = tabularData;
    }

    public TabularData TabularData => _tabularData;

    public override bool Equals(object? obj)
    {
        return obj is Instantiation instantiation &&
               _tabularData.Equals(instantiation._tabularData);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_tabularData);
    }
}
