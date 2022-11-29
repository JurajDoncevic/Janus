using Janus.Commons.SchemaModels;
using Janus.Commons.SelectionExpressions;
using System;
using Xunit;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.Commons.Tests;

public class SelectionExpressionTests
{
    [Fact(DisplayName = "Create a selection expression using EQ")]
    public void CreateEQExpression()
    {
        var attribute = AttributeId.From("dataSource.schema.tableau.attribute");
        int value = 2;
        SelectionExpression expression = EQ(attribute, value);

        Assert.Equal($"EQ({attribute},{value})", expression.ToString());
    }

    [Fact(DisplayName = "Create a selection expression using NEQ")]
    public void CreateNEQExpression()
    {
        var attribute = AttributeId.From("dataSource.schema.tableau.attribute");
        int value = 2;
        SelectionExpression expression = NEQ(attribute, value);

        Assert.Equal($"NEQ({attribute},{value})", expression.ToString());
    }

    [Fact(DisplayName = "Create a selection expression using GE")]
    public void CreateGEExpression()
    {
        var attribute = AttributeId.From("dataSource.schema.tableau.attribute");
        int value = 2;
        SelectionExpression expression = GE(attribute, value);

        Assert.Equal($"GE({attribute},{value})", expression.ToString());
    }

    [Fact(DisplayName = "Create a selection expression using GT")]
    public void CreateGTExpression()
    {
        var attribute = AttributeId.From("dataSource.schema.tableau.attribute");
        int value = 2;
        SelectionExpression expression = GT(attribute, value);

        Assert.Equal($"GT({attribute},{value})", expression.ToString());
    }

    [Fact(DisplayName = "Create a selection expression using LT")]
    public void CreateLTExpression()
    {
        var attribute = AttributeId.From("dataSource.schema.tableau.attribute");
        int value = 2;
        SelectionExpression expression = LT(attribute, value);

        Assert.Equal($"LT({attribute},{value})", expression.ToString());
    }

    [Fact(DisplayName = "Create a selection expression using LE")]
    public void CreateLEExpression()
    {
        var attribute = AttributeId.From("dataSource.schema.tableau.attribute");
        int value = 2;
        SelectionExpression expression = LE(attribute, value);

        Assert.Equal($"LE({attribute},{value})", expression.ToString());
    }

    [Fact(DisplayName = "Create a selection expression using OR")]
    public void CreateORExpression()
    {
        var attribute1 = AttributeId.From("ds.sc.tbl.attr1");
        int value1 = 1;
        var attribute2 = AttributeId.From("ds.sc.tbl.attr2");
        int value2 = 2;

        SelectionExpression expression =
            OR(EQ(attribute1, value1), EQ(attribute2, value2));

        Assert.Equal($"OR(EQ({attribute1},{value1}),EQ({attribute2},{value2}))", expression.ToString());
    }

    [Fact(DisplayName = "Create a selection expression using AND")]
    public void CreateANDExpression()
    {
        string attribute1 = "ds.sc.tbl.attr1";
        int value1 = 1;
        string attribute2 = "ds.sc.tbl.attr2";
        int value2 = 2;

        SelectionExpression expression =
            AND(EQ(attribute1, value1), EQ(attribute2, value2));

        Assert.Equal($"AND(EQ({attribute1},{value1}),EQ({attribute2},{value2}))", expression.ToString());
    }

    [Fact(DisplayName = "Create a selection expression using NOT")]
    public void CreateNOTExpression()
    {
        string attribute1 = "ds.sc.tbl.attr1";
        int value1 = 1;

        SelectionExpression expression =
            NOT(EQ(attribute1, value1));

        Assert.Equal($"NOT(EQ({attribute1},{value1}))", expression.ToString());
    }

    [Fact(DisplayName = "Create a complex expression")]
    public void CreateComplexExpression()
    {
        string attribute1 = "ds.sc.tbl.attr1";
        int value1 = 1;
        string attribute2 = "ds.sc.tbl.attr2";
        string value2 = "STRING_VAL";
        string attribute3 = "ds.sc.tbl.attr3";
        decimal value3 = 3.14m;
        string attribute4 = "ds.sc.tbl.attr4";
        DateTime value4 = new DateTime(2022, 5, 20);
        string attribute5 = "ds.sc.tbl.attr5";
        bool value5 = false;
        string attribute6 = "ds.sc.tbl.attr6";
        int value6 = 6;

        SelectionExpression expression =
            AND(
                OR(
                    AND(
                        OR(LT(attribute1, value1), EQ(attribute2, value2)),
                        OR(GE(attribute3, value3), EQ(attribute4, value4))
                        ),
                    EQ(attribute6, value6)
                    ),
                EQ(attribute5, value5)
                );


        Assert.Equal($"AND(OR(AND(OR(LT(ds.sc.tbl.attr1,{value1}),EQ(ds.sc.tbl.attr2,{value2})),OR(GE(ds.sc.tbl.attr3,{value3}),EQ(ds.sc.tbl.attr4,{value4}))),EQ(ds.sc.tbl.attr6,{value6})),EQ(ds.sc.tbl.attr5,{value5}))", expression.ToString());
    }
}
