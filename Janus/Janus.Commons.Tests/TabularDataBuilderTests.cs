﻿using Janus.Commons.DataModels;
using Janus.Commons.DataModels.Exceptions;
using Janus.Commons.SchemaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Janus.Commons.Tests;

public class TabularDataBuilderTests
{

    [Fact(DisplayName = "Create valid tabular data")]
    public void CreateValidTabularData()
    {
        var tabularData =
            TabularDataBuilder.InitTabularData(new Dictionary<string, DataTypes>
            {
                { "attrINT", DataTypes.INT },
                { "attrDECIMAL", DataTypes.DECIMAL },
                { "attrDATETIME", DataTypes.DATETIME },
                { "attrBOOLEAN", DataTypes.BOOLEAN },
                { "attrSTRING", DataTypes.STRING }
            })
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>
            {
                { "attrINT", 0 },
                { "attrDECIMAL", 2.0 },
                { "attrDATETIME", new DateTime(2022, 5, 27) },
                { "attrBOOLEAN", false },
                { "attrSTRING", "test_string1" }
            }))
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>
            {
                { "attrINT", 1 },
                { "attrDECIMAL", 2.1 },
                { "attrDATETIME", new DateTime(2022, 5, 28) },
                { "attrBOOLEAN", true },
                { "attrSTRING", "test_string2" }
            }))
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>
            {
                { "attrINT", 2 },
                { "attrDECIMAL", 2.2 },
                { "attrDATETIME", new DateTime(2022, 5, 29) },
                { "attrBOOLEAN", true },
                { "attrSTRING", "test_string3" }
            }))
            .Build();
        Type targetType = TypeMappings.MapToType(tabularData.AttributeDataTypes["attrDATETIME"]);

        Assert.NotNull(tabularData);
        Assert.Equal(3, tabularData.RowData.Count);
        Assert.Equal(new DateTime(2022, 5, 28), Convert.ChangeType(tabularData[1]["attrDATETIME"], targetType));
    }

    [Fact(DisplayName = "Create tabular data with invalid attribute in row")]
    public void CreateTabularDataWithInvalidAttributeInRow()
    {
        Assert.Throws<IncompatibleRowDataTypeException>(() =>
        {
            var tabularData =
            TabularDataBuilder.InitTabularData(new Dictionary<string, DataTypes>
            {
                        { "attrINT", DataTypes.INT },
                        { "attrDECIMAL", DataTypes.DECIMAL },
                        { "attrDATETIME", DataTypes.DATETIME },
                        { "attrBOOLEAN", DataTypes.BOOLEAN },
                        { "attrSTRING", DataTypes.STRING }
            })
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>
            {
                        { "attrINT", 0 },
                        { "attrDECIMAL", 2.0 },
                        { "attrDATETIME", new DateTime(2022, 5, 27) },
                        { "attrBOOLEAN", false },
                        { "attrSTRING", "test_string1" }
            }))
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>
            {
                        { "attrINT", 1 },
                        { "attrDECIMAL", 2.1 },
                        { "attr", new DateTime(2022, 5, 28) },
                        { "attrBOOLEAN", true },
                        { "attrSTRING", "test_string2" }
            }))
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>
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

    [Fact(DisplayName = "Create tabular data with invalid type in row")]
    public void CreateTabularDataWithInvalidTypeInRow()
    {
        Assert.Throws<IncompatibleDotNetTypeException>(() =>
        {
            var tabularData =
            TabularDataBuilder.InitTabularData(new Dictionary<string, DataTypes>
            {
                        { "attrINT", DataTypes.INT },
                        { "attrDECIMAL", DataTypes.DECIMAL },
                        { "attrDATETIME", DataTypes.DATETIME },
                        { "attrBOOLEAN", DataTypes.BOOLEAN },
                        { "attrSTRING", DataTypes.STRING }
            })
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>
            {
                        { "attrINT", 0 },
                        { "attrDECIMAL", 2.0 },
                        { "attrDATETIME", new DateTime(2022, 5, 27) },
                        { "attrBOOLEAN", false },
                        { "attrSTRING", "test_string1" }
            }))
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>
            {
                        { "attrINT", 1 },
                        { "attrDECIMAL", 2.1m },
                        { "attrDATETIME", new DateTime(2022, 5, 28) },
                        { "attrBOOLEAN", true },
                        { "attrSTRING", "test_string2" }
            }))
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>
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

    [Fact(DisplayName = "Round-trip serialization of tabular data")]
    public void TestJsonSerialization()
    {
        var tabularData =
            TabularDataBuilder.InitTabularData(new Dictionary<string, DataTypes>
            {
                        { "attrINT", DataTypes.INT },
                        { "attrDECIMAL", DataTypes.DECIMAL },
                        { "attrDATETIME", DataTypes.DATETIME },
                        { "attrBOOLEAN", DataTypes.BOOLEAN },
                        { "attrSTRING", DataTypes.STRING }
            })
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>
            {
                        { "attrINT", 0 },
                        { "attrDECIMAL", 2.0 },
                        { "attrDATETIME", new DateTime(2022, 5, 27) },
                        { "attrBOOLEAN", false },
                        { "attrSTRING", "test_string1" }
            }))
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>
            {
                        { "attrINT", 1 },
                        { "attrDECIMAL", 2.1 },
                        { "attrDATETIME", new DateTime(2022, 5, 28) },
                        { "attrBOOLEAN", true },
                        { "attrSTRING", "test_string2" }
            }))
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>
            {
                        { "attrINT", 2 },
                        { "attrDECIMAL", 2.2 },
                        { "attrDATETIME", new DateTime(2022, 5, 29) },
                        { "attrBOOLEAN", true },
                        { "attrSTRING", "test_string3" }
            }))
            .Build();

        var jsonString = System.Text.Json.JsonSerializer.Serialize(tabularData);
        var deserializedTabular = System.Text.Json.JsonSerializer.Deserialize<TabularData>(jsonString);

        Assert.Equal(tabularData, deserializedTabular);
    }

    [Fact(DisplayName = "Round-trip serialization of tabular data with a null value")]
    public void TestJsonSerializationWithNull()
    {
        var tabularData =
            TabularDataBuilder.InitTabularData(new Dictionary<string, DataTypes>
            {
                        { "attrINT", DataTypes.INT },
                        { "attrDECIMAL", DataTypes.DECIMAL },
                        { "attrDATETIME", DataTypes.DATETIME },
                        { "attrBOOLEAN", DataTypes.BOOLEAN },
                        { "attrSTRING", DataTypes.STRING }
            })
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>
            {
                        { "attrINT", 0 },
                        { "attrDECIMAL", 2.0 },
                        { "attrDATETIME", null },
                        { "attrBOOLEAN", false },
                        { "attrSTRING", "test_string1" }
            }))
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>
            {
                        { "attrINT", 1 },
                        { "attrDECIMAL", 2.1 },
                        { "attrDATETIME", new DateTime(2022, 5, 28) },
                        { "attrBOOLEAN", true },
                        { "attrSTRING", "test_string2" }
            }))
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>
            {
                        { "attrINT", 2 },
                        { "attrDECIMAL", 2.2 },
                        { "attrDATETIME", new DateTime(2022, 5, 29) },
                        { "attrBOOLEAN", true },
                        { "attrSTRING", "test_string3" }
            }))
            .Build();
        
        var jsonString = System.Text.Json.JsonSerializer.Serialize(tabularData);
        var deserializedTabular = System.Text.Json.JsonSerializer.Deserialize<TabularData>(jsonString);

        Assert.Equal(tabularData, deserializedTabular);
    }
}
