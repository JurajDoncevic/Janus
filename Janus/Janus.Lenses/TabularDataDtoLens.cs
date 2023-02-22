using Janus.Commons.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Lenses;
public class TabularDataDtoLens<TDto> : Lens<TabularData, IEnumerable<TDto>>
{
    private readonly RowDataDtoLens<TDto> _rowDataLens;

    internal TabularDataDtoLens() : base()
    {
        _rowDataLens = RowDataDtoLens.Construct<TDto>();
    }

    public override Func<IEnumerable<TDto>, TabularData, TabularData> Put =>
        (view, originalSource) => view.Map(viewItem => _rowDataLens.Put(viewItem, originalSource.RowData.First()))
                                      .Aggregate(TabularDataBuilder.InitTabularData(new Dictionary<string, Commons.SchemaModels.DataTypes>(originalSource.ColumnDataTypes)),
                                                 (acc, rowData) => acc.AddRow(conf => conf.WithRowData(new Dictionary<string, object?>(rowData.ColumnValues))))
                                      .WithName(originalSource.Name)
                                      .Build();

    public override Func<TabularData, IEnumerable<TDto>> Get =>
        (source) => source.RowData.Map(rd => _rowDataLens.Get(rd));

}

public static class TabularDataDtoLens
{
    public static TabularDataDtoLens<T> Construct<T>()
        => new TabularDataDtoLens<T>();
}
