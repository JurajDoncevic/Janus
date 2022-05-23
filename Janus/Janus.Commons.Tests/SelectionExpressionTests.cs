using Janus.Commons.SelectionExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Janus.Commons.SelectionExpressions.SelectionExpressions;

namespace Janus.Commons.Tests;

public class SelectionExpressionTests
{
    [Fact(DisplayName = "Create a selection expression using EQ")]
    public void CreateEQExpression()
    {
        string attribute = "dataSource.schema.tableau.attribute";
        int value = 2;
        SelectionExpression expression = EQ(attribute, value);

        Assert.Equal($"EQ({attribute},{value})", expression.ToString());
    }

    [Fact(DisplayName = "Create a selection expression using NEQ")]
    public void CreateNEQExpression()
    {
        string attribute = "dataSource.schema.tableau.attribute";
        int value = 2;
        SelectionExpression expression = NEQ(attribute, value);

        Assert.Equal($"NEQ({attribute},{value})", expression.ToString());
    }

    [Fact(DisplayName = "Create a selection expression using GE")]
    public void CreateGEExpression()
    {
        string attribute = "dataSource.schema.tableau.attribute";
        int value = 2;
        SelectionExpression expression = GE(attribute, value);

        Assert.Equal($"GE({attribute},{value})", expression.ToString());
    }

    [Fact(DisplayName = "Create a selection expression using GT")]
    public void CreateGTExpression()
    {
        string attribute = "dataSource.schema.tableau.attribute";
        int value = 2;
        SelectionExpression expression = GT(attribute, value);

        Assert.Equal($"GT({attribute},{value})", expression.ToString());
    }

    [Fact(DisplayName = "Create a selection expression using LT")]
    public void CreateLTExpression()
    {
        string attribute = "dataSource.schema.tableau.attribute";
        int value = 2;
        SelectionExpression expression = LT(attribute, value);

        Assert.Equal($"LT({attribute},{value})", expression.ToString());
    }

    [Fact(DisplayName = "Create a selection expression using LE")]
    public void CreateLEExpression()
    {
        string attribute = "dataSource.schema.tableau.attribute";
        int value = 2;
        SelectionExpression expression = LE(attribute, value);

        Assert.Equal($"LE({attribute},{value})", expression.ToString());
    }

    [Fact(DisplayName = "Create a selection expression using OR")]
    public void CreateORExpression()
    {
        string attribute1 = "attr1";
        int value1 = 1;
        string attribute2 = "attr2";
        int value2 = 2;

        SelectionExpression expression =
            OR(EQ(attribute1, value1), EQ(attribute2, value2));

        Assert.Equal($"OR(EQ({attribute1},{value1}),EQ({attribute2},{value2}))", expression.ToString());
    }

    [Fact(DisplayName = "Create a selection expression using AND")]
    public void CreateANDExpression()
    {
        string attribute1 = "attr1";
        int value1 = 1;
        string attribute2 = "attr2";
        int value2 = 2;

        SelectionExpression expression =
            AND(EQ(attribute1, value1), EQ(attribute2, value2));

        Assert.Equal($"AND(EQ({attribute1},{value1}),EQ({attribute2},{value2}))", expression.ToString());
    }

    [Fact(DisplayName = "Create a selection expression using NOT")]
    public void CreateNOTExpression()
    {
        string attribute1 = "attr1";
        int value1 = 1;

        SelectionExpression expression =
            NOT(EQ(attribute1, value1));

        Assert.Equal($"NOT(EQ({attribute1},{value1}))", expression.ToString());
    }

    [Fact(DisplayName = "Create a complex expression")]
    public void CreateComplexExpression()
    {
        string attribute1 = "attr1";
        int value1 = 1;
        string attribute2 = "attr2";
        string value2 = "STRING_VAL";
        string attribute3 = "attr3";
        decimal value3 = 3.14m;
        string attribute4 = "attr4";
        DateTime value4 = new DateTime(2022, 5, 20);
        string attribute5 = "attr5";
        bool value5 = false;
        string attribute6 = "attr6";
        int value6 = 6;

        SelectionExpression expression =
            AND(OR(AND(OR(LT(attribute1, value1),EQ(attribute2, value2)), OR(GE(attribute3, value3), EQ(attribute4, value4))), EQ(attribute6, value6)), EQ(attribute5, value5));


        Assert.Equal("AND(OR(AND(OR(LT(attr1,1),EQ(attr2,STRING_VAL)),OR(GE(attr3,3,14),EQ(attr4,20.5.2022. 0:00:00))),EQ(attr6,6)),EQ(attr5,False))", expression.ToString());
    }
}
