using Janus.Commons.DataModels;
using Janus.Commons.DataModels.Exceptions;
using Janus.Commons.SchemaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Janus.Commons.Tests;

public class TableauDataBuilderTests
{

    [Fact(DisplayName = "Create valid tableau data")]
    public void CreateValidTableauData()
    {
        var tableauData =
            TableauDataBuilder.InitTableauData(new Dictionary<string, DataTypes>
            {
                { "attrINT", DataTypes.INT },
                { "attrDECIMAL", DataTypes.DECIMAL },
                { "attrDATETIME", DataTypes.DATETIME },
                { "attrBOOLEAN", DataTypes.BOOLEAN },
                { "attrSTRING", DataTypes.STRING }
            })
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object>
            {
                { "attrINT", 0 },
                { "attrDECIMAL", 2.0 },
                { "attrDATETIME", new DateTime(2022, 5, 27) },
                { "attrBOOLEAN", false },
                { "attrSTRING", "test_string1" }
            }))
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object>
            {
                { "attrINT", 1 },
                { "attrDECIMAL", 2.1 },
                { "attrDATETIME", new DateTime(2022, 5, 28) },
                { "attrBOOLEAN", true },
                { "attrSTRING", "test_string2" }
            }))
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object>
            {
                { "attrINT", 2 },
                { "attrDECIMAL", 2.2 },
                { "attrDATETIME", new DateTime(2022, 5, 29) },
                { "attrBOOLEAN", true },
                { "attrSTRING", "test_string3" }
            }))
            .Build();
        Type targetType = TypeMappings.MapToType(tableauData.AttributeDataTypes["attrDATETIME"]);

        Assert.NotNull(tableauData);
        Assert.Equal(3, tableauData.RowData.Count);
        Assert.Equal(new DateTime(2022, 5, 28), Convert.ChangeType(tableauData[1]["attrDATETIME"], targetType));
    }

    [Fact(DisplayName = "Create tableau data with invalid attribute in row")]
    public void CreateTableauDataWithInvalidAttributeInRow()
    {
        Assert.Throws<IncompatibleRowDataTypeException>(() =>
        {
            var tableauData =
            TableauDataBuilder.InitTableauData(new Dictionary<string, DataTypes>
            {
                        { "attrINT", DataTypes.INT },
                        { "attrDECIMAL", DataTypes.DECIMAL },
                        { "attrDATETIME", DataTypes.DATETIME },
                        { "attrBOOLEAN", DataTypes.BOOLEAN },
                        { "attrSTRING", DataTypes.STRING }
            })
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object>
            {
                        { "attrINT", 0 },
                        { "attrDECIMAL", 2.0 },
                        { "attrDATETIME", new DateTime(2022, 5, 27) },
                        { "attrBOOLEAN", false },
                        { "attrSTRING", "test_string1" }
            }))
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object>
            {
                        { "attrINT", 1 },
                        { "attrDECIMAL", 2.1 },
                        { "attr", new DateTime(2022, 5, 28) },
                        { "attrBOOLEAN", true },
                        { "attrSTRING", "test_string2" }
            }))
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object>
            {
                        { "attrINT", 2 },
                        { "attrDECIMAL", 2.2 },
                        { "attrDATETIME", new DateTime(2022, 5, 29) },
                        { "attrBOOLEAN", true },
                        { "attrSTRING", "test_string3" }
            }))
            .Build();
        });
    }

    [Fact(DisplayName = "Create tableau data with invalid type in row")]
    public void CreateTableauDataWithInvalidTypeInRow()
    {
        Assert.Throws<IncompatibleDotNetTypeException>(() =>
        {
            var tableauData =
            TableauDataBuilder.InitTableauData(new Dictionary<string, DataTypes>
            {
                        { "attrINT", DataTypes.INT },
                        { "attrDECIMAL", DataTypes.DECIMAL },
                        { "attrDATETIME", DataTypes.DATETIME },
                        { "attrBOOLEAN", DataTypes.BOOLEAN },
                        { "attrSTRING", DataTypes.STRING }
            })
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object>
            {
                        { "attrINT", 0 },
                        { "attrDECIMAL", 2.0 },
                        { "attrDATETIME", new DateTime(2022, 5, 27) },
                        { "attrBOOLEAN", false },
                        { "attrSTRING", "test_string1" }
            }))
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object>
            {
                        { "attrINT", 1 },
                        { "attrDECIMAL", 2.1m },
                        { "attrDATETIME", new DateTime(2022, 5, 28) },
                        { "attrBOOLEAN", true },
                        { "attrSTRING", "test_string2" }
            }))
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object>
            {
                        { "attrINT", 2 },
                        { "attrDECIMAL", 2.2 },
                        { "attrDATETIME", new DateTime(2022, 5, 29) },
                        { "attrBOOLEAN", true },
                        { "attrSTRING", "test_string3" }
            }))
            .Build();
        });
    }
}
