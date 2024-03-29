﻿using Janus.Commons.DataModels;

namespace Janus.Commons.CommandModels;

/// <summary>
/// Describes the instantiation clause
/// </summary>
public sealed class Instantiation
{
    private readonly TabularData _tabularData;

    /// <summary>
    /// Tabular data used in the instantiation. Only attribute names are used for column names
    /// </summary>
    public TabularData TabularData => _tabularData;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="tabularData">Tabular data to use for instantiation. Use attribute names for column names</param>
    internal Instantiation(TabularData tabularData)
    {
        _tabularData = tabularData ?? throw new ArgumentNullException(nameof(tabularData));
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
