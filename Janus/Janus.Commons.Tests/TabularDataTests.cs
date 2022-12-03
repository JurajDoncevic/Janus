using Janus.Commons.DataModels;
using Janus.Commons.SchemaModels;
using static Janus.Commons.SelectionExpressions.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Janus.Commons.Tests;
public class TabularDataTests
{
    private readonly TabularData[] _tabularData =
    {
        TabularDataBuilder.InitTabularData(new Dictionary<string, DataTypes>
            {
                { "ds1.sc.tbl.attrINT", DataTypes.INT },
                { "ds1.sc.tbl.attrDECIMAL", DataTypes.DECIMAL },
                { "ds1.sc.tbl.attrDATETIME", DataTypes.DATETIME },
                { "ds1.sc.tbl.attrBOOLEAN", DataTypes.BOOLEAN },
                { "ds1.sc.tbl.attrSTRING", DataTypes.STRING },
                { "ds1.sc.tbl.fkINT", DataTypes.INT },
            })
            .WithName("DS1_testData")
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>
            {
                { "ds1.sc.tbl.attrINT", 0 },
                { "ds1.sc.tbl.attrDECIMAL", 2.0 },
                { "ds1.sc.tbl.attrDATETIME", new DateTime(2022, 5, 27) },
                { "ds1.sc.tbl.attrBOOLEAN", false },
                { "ds1.sc.tbl.attrSTRING", "test_string1" },
                { "ds1.sc.tbl.fkINT", 2 },
            }))
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>
            {
                { "ds1.sc.tbl.attrINT", 1 },
                { "ds1.sc.tbl.attrDECIMAL", 2.1 },
                { "ds1.sc.tbl.attrDATETIME", new DateTime(2022, 5, 28) },
                { "ds1.sc.tbl.attrBOOLEAN", true },
                { "ds1.sc.tbl.attrSTRING", "test_string2" },
                { "ds1.sc.tbl.fkINT", 2 },
            }))
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>
            {
                { "ds1.sc.tbl.attrINT", 2 },
                { "ds1.sc.tbl.attrDECIMAL", 2.2 },
                { "ds1.sc.tbl.attrDATETIME", new DateTime(2022, 5, 29) },
                { "ds1.sc.tbl.attrBOOLEAN", true },
                { "ds1.sc.tbl.attrSTRING", "test_string3" },
                { "ds1.sc.tbl.fkINT", 1 },
            }))
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>
            {
                { "ds1.sc.tbl.attrINT", 2 },
                { "ds1.sc.tbl.attrDECIMAL", 2.2 },
                { "ds1.sc.tbl.attrDATETIME", new DateTime(2022, 5, 29) },
                { "ds1.sc.tbl.attrBOOLEAN", true },
                { "ds1.sc.tbl.attrSTRING", "test_string3" },
                { "ds1.sc.tbl.fkINT", -1 },
            })).Build(),
        TabularDataBuilder.InitTabularData(new Dictionary<string, DataTypes>
            {
                { "ds2.attrINT", DataTypes.INT },
                { "ds2.attrDECIMAL", DataTypes.DECIMAL },
                { "ds2.attrDATETIME", DataTypes.DATETIME },
                { "ds2.attrBOOLEAN", DataTypes.BOOLEAN },
                { "ds2.attrSTRING", DataTypes.STRING }
            })
            .WithName("DS2_testData")
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>
            {
                { "ds2.attrINT", 0 },
                { "ds2.attrDECIMAL", 2.0 },
                { "ds2.attrDATETIME", new DateTime(2022, 5, 27) },
                { "ds2.attrBOOLEAN", false },
                { "ds2.attrSTRING", "test_string1" }
            }))
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>
            {
                { "ds2.attrINT", 1 },
                { "ds2.attrDECIMAL", 2.1 },
                { "ds2.attrDATETIME", new DateTime(2022, 5, 28) },
                { "ds2.attrBOOLEAN", true },
                { "ds2.attrSTRING", "test_string2" }
            }))
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>
            {
                { "ds2.attrINT", 2 },
                { "ds2.attrDECIMAL", 2.2 },
                { "ds2.attrDATETIME", new DateTime(2022, 5, 29) },
                { "ds2.attrBOOLEAN", true },
                { "ds2.attrSTRING", "test_string3" }
            })).Build(),

    };
    [Fact(DisplayName = "Create a joined tabular data")]
    public void EquiJoinTabularDataTest()
    {
        var joiningResult = TabularDataOperations.EquiJoinTabularData(_tabularData[0], _tabularData[1], "ds1.sc.tbl.fkINT", "ds2.attrINT");


        Assert.True(joiningResult);
    }

    [Fact(DisplayName = "Project columns of a tabular data")]
    public void ProjectColumnsOfTabularDataTest()
    {
        var columnsToProject = new HashSet<string> { "ds1.sc.tbl.attrINT", "ds1.sc.tbl.attrDECIMAL", "ds1.sc.tbl.attrDATETIME" };

        var projectionResult = TabularDataOperations.ProjectColumns(_tabularData[0], columnsToProject);
        Assert.True(projectionResult);
        Assert.Equal(projectionResult.Data.ColumnNames, columnsToProject);
        Assert.Equal(projectionResult.Data.RowData.First().ColumnValues.Keys.ToHashSet(), columnsToProject);
    }

    [Fact(DisplayName = "Select rows of a tabular data")]
    public void SelectRowsOfTabularDataTest()
    {
        var selectionExpression = GT("ds1.sc.tbl.attrDECIMAL", 2.1);

        var selectionResult = TabularDataOperations.SelectRowData(_tabularData[0], selectionExpression);

        var rowsExpected = _tabularData[0].RowData.Where(rd => (double)rd.ColumnValues["ds1.sc.tbl.attrDECIMAL"] > 2.1);

        Assert.True(selectionResult);
        Assert.Equal(rowsExpected, selectionResult.Data.RowData);
    }

    [Fact(DisplayName = "Rename columns of a tabular data")]
    public void RenameColumnsOfTabularDataTest()
    {
        var columnRenames = new Dictionary<string, string> 
        {
                { "ds1.sc.tbl.attrINT", "ds1.sc.tbl.attrINT_renamed" },
                { "ds1.sc.tbl.attrDECIMAL", "ds1.sc.tbl.attrDECIMAL_renamed" },
                { "ds1.sc.tbl.attrDATETIME", "ds1.sc.tbl.attrDATETIME_renamed" },
                { "ds1.sc.tbl.attrBOOLEAN", "ds1.sc.tbl.attrBOOLEAN_renamed" },
                { "ds1.sc.tbl.attrSTRING", "ds1.sc.tbl.attrSTRING_renamed" },
                { "ds1.sc.tbl.fkINT", "ds1.sc.tbl.fkINT_renamed" }
        };

        var renameResult = TabularDataOperations.RenameColumns(_tabularData[0], columnRenames);
        Assert.True(renameResult);
        Assert.Equal(renameResult.Data.ColumnNames.ToHashSet(), columnRenames.Values.ToHashSet());
    }
}
