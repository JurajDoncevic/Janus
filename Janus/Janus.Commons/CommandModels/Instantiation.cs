using Janus.Commons.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.CommandModels;

/// <summary>
/// Describes the instantiation clause
/// </summary>
public class Instantiation
{
    private readonly TabularData _tabularData;

    /// <summary>
    /// Tabular data used in the instantiation
    /// </summary>
    public TabularData TabularData => _tabularData;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="tabularData">Tabular data to use</param>
    internal Instantiation(TabularData tabularData!!)
    {
        _tabularData = tabularData;
    }

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
