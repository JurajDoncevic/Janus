using Janus.Commons.DataModels;
using Janus.Commons.SchemaModels;
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
                { "ds1.attrINT", DataTypes.INT },
                { "ds1.attrDECIMAL", DataTypes.DECIMAL },
                { "ds1.attrDATETIME", DataTypes.DATETIME },
                { "ds1.attrBOOLEAN", DataTypes.BOOLEAN },
                { "ds1.attrSTRING", DataTypes.STRING },
                { "ds1.fkINT", DataTypes.INT },
            })
            .WithName("DS1_testData")
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>
            {
                { "ds1.attrINT", 0 },
                { "ds1.attrDECIMAL", 2.0 },
                { "ds1.attrDATETIME", new DateTime(2022, 5, 27) },
                { "ds1.attrBOOLEAN", false },
                { "ds1.attrSTRING", "test_string1" },
                { "ds1.fkINT", 2 },
            }))
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>
            {
                { "ds1.attrINT", 1 },
                { "ds1.attrDECIMAL", 2.1 },
                { "ds1.attrDATETIME", new DateTime(2022, 5, 28) },
                { "ds1.attrBOOLEAN", true },
                { "ds1.attrSTRING", "test_string2" },
                { "ds1.fkINT", 2 },
            }))
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>
            {
                { "ds1.attrINT", 2 },
                { "ds1.attrDECIMAL", 2.2 },
                { "ds1.attrDATETIME", new DateTime(2022, 5, 29) },
                { "ds1.attrBOOLEAN", true },
                { "ds1.attrSTRING", "test_string3" },
                { "ds1.fkINT", 1 },
            }))
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>
            {
                { "ds1.attrINT", 2 },
                { "ds1.attrDECIMAL", 2.2 },
                { "ds1.attrDATETIME", new DateTime(2022, 5, 29) },
                { "ds1.attrBOOLEAN", true },
                { "ds1.attrSTRING", "test_string3" },
                { "ds1.fkINT", -1 },
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
        var joiningResult = TabularDataOperations.EquiJoinTabularData(_tabularData[0], _tabularData[1], "ds1.fkINT", "ds2.attrINT");


        Assert.True(joiningResult);
    }

    [Fact(DisplayName = "Project columns of a tabular data")]
    public void ProjectColumnsOfTabularDataTest()
    {
        var columnsToProject = new HashSet<string> { "ds1.attrINT", "ds1.attrDECIMAL", "ds1.attrDATETIME" };

        var projectionResult = TabularDataOperations.ProjectColumns(_tabularData[0], columnsToProject);
        Assert.True(projectionResult);
        Assert.Equal(projectionResult.Data.ColumnNames, columnsToProject);
        Assert.Equal(projectionResult.Data.RowData.First().ColumnValues.Keys.ToHashSet(), columnsToProject);
    }
}
