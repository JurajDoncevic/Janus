using Janus.Commons.SchemaModels;
using Janus.Commons.SchemaModels.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Janus.Commons.Tests;
public class ElementIdentifierTests
{
    [Fact(DisplayName = "Create a data source identifier")]
    public void CreateDataSourceId()
    {
        var fromString = DataSourceId.From("ds1");
        var fromNames = DataSourceId.From("ds1");

        Assert.Equal(fromString, fromNames);
    }

    [Fact(DisplayName = "Fail to create a data source identifier due to faulty string representation")]
    public void FailCreateDataSourceId()
    {
        Assert.Throws<ArgumentException>(() => DataSourceId.From("ds1.something"));
        Assert.Throws<ArgumentException>(() => DataSourceId.From(""));
    }

    [Fact(DisplayName = "Create a schema identifier")]
    public void CreateSchemaId()
    {
        var parentDataSourceId = DataSourceId.From("ds1");

        var fromString = SchemaId.From("ds1.sc1");
        var fromNames = SchemaId.From("ds1", "sc1");

        Assert.Equal(fromString, fromNames);
        Assert.True(parentDataSourceId.IsParentOf(fromNames));
        Assert.True(fromString.IsChildOf(parentDataSourceId));
    }

    [Fact(DisplayName = "Fail to create a schema identifier due to faulty string representation")]
    public void FailCreateSchemaId()
    {
        Assert.Throws<InvalidIdStringException>(() => SchemaId.From("ds1sc1"));
        Assert.Throws<InvalidIdStringException>(() => SchemaId.From("ds1.sc1.something"));
        Assert.Throws<InvalidIdStringException>(() => SchemaId.From(""));
    }

    [Fact(DisplayName = "Create an tableau identifier")]
    public void CreateTableauId()
    {
        var parentDataSourceId = DataSourceId.From("ds1");
        var parentSchemaId = SchemaId.From("ds1", "sc1");

        var fromString = TableauId.From("ds1.sc1.tbl1");
        var fromNames = TableauId.From("ds1", "sc1", "tbl1");

        Assert.Equal(fromString, fromNames);
        Assert.True(parentDataSourceId.IsParentOf(fromNames));
        Assert.True(parentSchemaId.IsParentOf(fromNames));
        Assert.True(fromString.IsChildOf(parentDataSourceId));
        Assert.True(fromString.IsChildOf(parentSchemaId));
    }

    [Fact(DisplayName = "Fail to create a tableau identifier due to faulty string representation")]
    public void FailCreateTableauId()
    {
        Assert.Throws<InvalidIdStringException>(() => TableauId.From("ds1.sc1tbl1"));
        Assert.Throws<InvalidIdStringException>(() => TableauId.From("ds1.sc1.tbl1.something"));
        Assert.Throws<InvalidIdStringException>(() => TableauId.From(""));
    }

    [Fact(DisplayName = "Create an attribute identifier")]
    public void CreateAttributeId()
    {
        var parentDataSourceId = DataSourceId.From("ds1");
        var parentSchemaId = SchemaId.From("ds1", "sc1");
        var parentTableauId = TableauId.From("ds1", "sc1", "tbl1");

        var fromString = AttributeId.From("ds1.sc1.tbl1.attr1");
        var fromNames = AttributeId.From("ds1", "sc1", "tbl1", "attr1");

        Assert.Equal(fromString, fromNames);
        Assert.True(parentDataSourceId.IsParentOf(fromNames));
        Assert.True(parentSchemaId.IsParentOf(fromNames));
        Assert.True(parentTableauId.IsParentOf(fromNames));
        Assert.True(fromString.IsChildOf(parentDataSourceId));
        Assert.True(fromString.IsChildOf(parentSchemaId));
        Assert.True(fromString.IsChildOf(parentTableauId));
    }

    [Fact(DisplayName = "Fail to create an attribute identifier due to faulty string representation")]
    public void FailCreateAttributeId()
    {
        Assert.Throws<InvalidIdStringException>(() => AttributeId.From("ds1.sc1.tbl1attr1"));
        Assert.Throws<InvalidIdStringException>(() => AttributeId.From("ds1.sc1.tbl1.attr1.something"));
        Assert.Throws<InvalidIdStringException>(() => AttributeId.From(""));
    }
}
