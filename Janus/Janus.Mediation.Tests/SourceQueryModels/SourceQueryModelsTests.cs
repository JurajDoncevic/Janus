using Janus.Commons.SchemaModels;
using Janus.Mediation.SchemaMediationModels.Exceptions;
using Janus.Mediation.SchemaMediationModels.SourceQueryModels.Building;

namespace Janus.Mediation.Tests.SourceQueryModels;
public class SourceQueryModelsTests
{
    private readonly DataSource[] _dataSources =
        {
            SchemaModelBuilder.InitDataSource("ds1")
                .AddSchema("sc1", conf => 
                    conf.AddTableau("tbl1", conf => 
                            conf.AddAttribute("attr1", conf => 
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.INT))
                                .AddAttribute("attr2", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.STRING))
                                .AddAttribute("attr3", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.INT)))
                        .AddTableau("tbl2", conf =>
                            conf.AddAttribute("attr1", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.INT))
                                .AddAttribute("attr2", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.STRING)))
                        .AddTableau("tbl3", conf =>
                            conf.AddAttribute("attr1", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.INT))
                                .AddAttribute("attr2", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.DECIMAL))))
                .AddSchema("sc2", conf =>
                    conf.AddTableau("tbl1", conf =>
                            conf.AddAttribute("attr1", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.INT))
                                .AddAttribute("attr2", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.STRING))
                                .AddAttribute("attr3", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.STRING)))
                        .AddTableau("tbl2", conf =>
                            conf.AddAttribute("attr1", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.INT))
                                .AddAttribute("attr2", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.STRING)))
                        .AddTableau("tbl3", conf =>
                            conf.AddAttribute("attr1", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.INT))
                                .AddAttribute("attr2", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.DECIMAL))))
                .Build(),
            SchemaModelBuilder.InitDataSource("ds2")
                .AddSchema("sc1", conf => 
                    conf.AddTableau("tbl1", conf => 
                            conf.AddAttribute("attr1", conf => 
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.INT))
                                .AddAttribute("attr2", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.STRING))
                                .AddAttribute("attr3", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.STRING)))
                        .AddTableau("tbl2", conf =>
                            conf.AddAttribute("attr1", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.INT))
                                .AddAttribute("attr2", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.STRING)))
                        .AddTableau("tbl3", conf =>
                            conf.AddAttribute("attr1", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.INT))
                                .AddAttribute("attr2", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.DECIMAL))))
                .AddSchema("sc2", conf =>
                    conf.AddTableau("tbl1", conf =>
                            conf.AddAttribute("attr1", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.INT))
                                .AddAttribute("attr2", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.STRING))
                                .AddAttribute("attr3", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.STRING)))
                        .AddTableau("tbl2", conf =>
                            conf.AddAttribute("attr1", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.INT))
                                .AddAttribute("attr2", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.STRING)))
                        .AddTableau("tbl3", conf =>
                            conf.AddAttribute("attr1", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.INT))
                                .AddAttribute("attr2", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.DECIMAL))))
                .Build(),
            SchemaModelBuilder.InitDataSource("ds3")
                .AddSchema("sc1", conf => 
                    conf.AddTableau("tbl1", conf => 
                            conf.AddAttribute("attr1", conf => 
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.INT))
                                .AddAttribute("attr2", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.STRING)))
                        .AddTableau("tbl2", conf =>
                            conf.AddAttribute("attr1", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.INT))
                                .AddAttribute("attr2", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.STRING)))
                        .AddTableau("tbl3", conf =>
                            conf.AddAttribute("attr1", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.INT))
                                .AddAttribute("attr2", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.DECIMAL))))
                .AddSchema("sc2", conf =>
                    conf.AddTableau("tbl1", conf =>
                            conf.AddAttribute("attr1", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.INT))
                                .AddAttribute("attr2", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.STRING)))
                        .AddTableau("tbl2", conf =>
                            conf.AddAttribute("attr1", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.INT))
                                .AddAttribute("attr2", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.STRING)))
                        .AddTableau("tbl3", conf =>
                            conf.AddAttribute("attr1", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.INT))
                                .AddAttribute("attr2", conf =>
                                conf.WithIsNullable(false)
                                    .WithDataType(DataTypes.DECIMAL))))
                .Build()
        };

    [Fact(DisplayName = "Build source query with no joins")]
    public void BuildSimpleSourceQueryTest()
    {
        var sourceQuery =
        SourceQueryBuilder.InitQueryOn("ds1.sc1.tbl1", _dataSources.ToList())
            .WithProjection(conf => conf.AddAttribute("ds1.sc1.tbl1.attr1")
                                        .AddAttribute("ds1.sc1.tbl1.attr3"))
            .Build();

        Assert.NotNull(sourceQuery);
        Assert.Contains("ds1.sc1.tbl1.attr1", sourceQuery.Projection.IncludedAttributeIds);
        Assert.Contains("ds1.sc1.tbl1.attr3", sourceQuery.Projection.IncludedAttributeIds);
        Assert.DoesNotContain("ds1.sc1.tbl1.attr2", sourceQuery.Projection.IncludedAttributeIds);
        Assert.Equal("ds1.sc1.tbl1", sourceQuery.InitialTableauId);
    }

    [Fact(DisplayName = "Build source query with joins from the same data source")]
    public void BuildSourceQueryWithLocalJoinsTest()
    {
        var sourceQuery =
        SourceQueryBuilder.InitQueryOn("ds1.sc1.tbl1", _dataSources.ToList())
            .WithJoining(conf => conf.AddJoin("ds1.sc1.tbl1.attr3", "ds1.sc1.tbl2.attr1"))
            .WithProjection(conf => conf.AddAttribute("ds1.sc1.tbl1.attr1")
                                        .AddAttribute("ds1.sc1.tbl1.attr2")
                                        .AddAttribute("ds1.sc1.tbl1.attr3")
                                        .AddAttribute("ds1.sc1.tbl2.attr1")
                                        .AddAttribute("ds1.sc1.tbl2.attr2"))
            .Build();

        Assert.NotNull(sourceQuery);
        Assert.Contains("ds1.sc1.tbl1.attr1", sourceQuery.Projection.IncludedAttributeIds);
        Assert.Contains("ds1.sc1.tbl1.attr2", sourceQuery.Projection.IncludedAttributeIds);
        Assert.Contains("ds1.sc1.tbl1.attr3", sourceQuery.Projection.IncludedAttributeIds);
        Assert.Contains("ds1.sc1.tbl2.attr1", sourceQuery.Projection.IncludedAttributeIds);
        Assert.Contains("ds1.sc1.tbl2.attr2", sourceQuery.Projection.IncludedAttributeIds);
        Assert.True(sourceQuery.Joining);
        Assert.Equal(1, sourceQuery.Joining.Value.Joins.Count);
        Assert.Equal("ds1.sc1.tbl1", sourceQuery.InitialTableauId);
    }

    [Fact(DisplayName = "Build source query with joins from different data sources")]
    public void BuildSourceQueryWithJoinsTest()
    {
        var sourceQuery =
        SourceQueryBuilder.InitQueryOn("ds1.sc1.tbl1", _dataSources.ToList())
            .WithJoining(conf => conf.AddJoin("ds1.sc1.tbl1.attr3", "ds2.sc1.tbl1.attr1")
                                     .AddJoin("ds1.sc1.tbl1.attr3", "ds3.sc1.tbl1.attr1"))
            .WithProjection(conf => conf.AddAttribute("ds1.sc1.tbl1.attr1")
                                        .AddAttribute("ds1.sc1.tbl1.attr2")
                                        .AddAttribute("ds1.sc1.tbl1.attr3")
                                        .AddAttribute("ds2.sc1.tbl1.attr1")
                                        .AddAttribute("ds2.sc1.tbl1.attr2")
                                        .AddAttribute("ds3.sc1.tbl1.attr1")
                                        .AddAttribute("ds3.sc1.tbl1.attr2"))
            .Build();

        Assert.NotNull(sourceQuery);
        Assert.Contains("ds1.sc1.tbl1.attr1", sourceQuery.Projection.IncludedAttributeIds);
        Assert.Contains("ds1.sc1.tbl1.attr2", sourceQuery.Projection.IncludedAttributeIds);
        Assert.Contains("ds1.sc1.tbl1.attr3", sourceQuery.Projection.IncludedAttributeIds);
        Assert.Contains("ds2.sc1.tbl1.attr1", sourceQuery.Projection.IncludedAttributeIds);
        Assert.Contains("ds2.sc1.tbl1.attr2", sourceQuery.Projection.IncludedAttributeIds);
        Assert.Contains("ds3.sc1.tbl1.attr1", sourceQuery.Projection.IncludedAttributeIds);
        Assert.Contains("ds3.sc1.tbl1.attr2", sourceQuery.Projection.IncludedAttributeIds);
        Assert.True(sourceQuery.Joining);
        Assert.Equal(2, sourceQuery.Joining.Value.Joins.Count);
        Assert.Equal("ds1.sc1.tbl1", sourceQuery.InitialTableauId);
    }

    [Fact(DisplayName = "Fail to build source query with unknown attribute")]
    public void BuildSourceQueryWithNonExistingAttributeInProjection()
    {
        Assert.Throws<AttributeDoesNotExistException>(() =>
        {
            var sourceQuery =
            SourceQueryBuilder.InitQueryOn("ds1.sc1.tbl1", _dataSources.ToList())
                .WithJoining(conf => conf.AddJoin("ds1.sc1.tbl1.attr3", "ds2.sc1.tbl1.attr1")
                                         .AddJoin("ds1.sc1.tbl1.attr3", "ds3.sc1.tbl1.attr1"))
                .WithProjection(conf => conf.AddAttribute("ds1.sc1.tbl1.attr1")
                                            .AddAttribute("ds1.sc1.tbl1.attr2")
                                            .AddAttribute("ds1.sc1.tbl1.attr4") // does not exist
                                            .AddAttribute("ds2.sc1.tbl1.attr1")
                                            .AddAttribute("ds2.sc1.tbl1.attr2")
                                            .AddAttribute("ds3.sc1.tbl1.attr1")
                                            .AddAttribute("ds3.sc1.tbl1.attr2"))
                .Build();
        });
    }

    [Fact(DisplayName = "Fail to build source query with self-join")]
    public void BuildSourceQueryWithSelfJoin()
    {
        Assert.Throws<SelfJoinNotSupportedException>(() =>
        {
            var sourceQuery =
            SourceQueryBuilder.InitQueryOn("ds1.sc1.tbl1", _dataSources.ToList())
                .WithJoining(conf => conf.AddJoin("ds1.sc1.tbl1.attr3", "ds1.sc1.tbl1.attr1"))
                .WithProjection(conf => conf.AddAttribute("ds1.sc1.tbl1.attr1")
                                            .AddAttribute("ds1.sc1.tbl1.attr2")
                                            .AddAttribute("ds1.sc1.tbl1.attr3"))
                .Build();
        });
    }

    [Fact(DisplayName = "Fail to build source query with cyclic join")]
    public void BuildSourceQueryWithCyclicJoin()
    {
        Assert.Throws<CyclicJoinNotSupportedException>(() =>
        {
            var sourceQuery =
            SourceQueryBuilder.InitQueryOn("ds1.sc1.tbl1", _dataSources.ToList())
                .WithJoining(conf => conf.AddJoin("ds1.sc1.tbl1.attr3", "ds2.sc1.tbl1.attr1")
                                         .AddJoin("ds1.sc1.tbl1.attr3", "ds3.sc1.tbl1.attr1")
                                         .AddJoin("ds3.sc1.tbl1.attr1", "ds1.sc1.tbl1.attr1"))
                .WithProjection(conf => conf.AddAttribute("ds1.sc1.tbl1.attr1")
                                            .AddAttribute("ds1.sc1.tbl1.attr2")
                                            .AddAttribute("ds2.sc1.tbl1.attr1")
                                            .AddAttribute("ds2.sc1.tbl1.attr2")
                                            .AddAttribute("ds3.sc1.tbl1.attr1")
                                            .AddAttribute("ds3.sc1.tbl1.attr2"))
                .Build();
        });
    }

    [Fact(DisplayName = "Fail to build source query with wrong join types")]
    public void BuildSourceQueryWithWrongJoinDataTypes()
    {
        Assert.Throws<JoinedAttributesNotOfSameTypeException>(() =>
        {
            var sourceQuery =
            SourceQueryBuilder.InitQueryOn("ds1.sc1.tbl1", _dataSources.ToList())
                .WithJoining(conf => conf.AddJoin("ds1.sc1.tbl1.attr3", "ds2.sc1.tbl1.attr1")
                                         .AddJoin("ds1.sc1.tbl1.attr3", "ds3.sc1.tbl1.attr2"))
                .WithProjection(conf => conf.AddAttribute("ds1.sc1.tbl1.attr1")
                                            .AddAttribute("ds1.sc1.tbl1.attr2")
                                            .AddAttribute("ds2.sc1.tbl1.attr1")
                                            .AddAttribute("ds2.sc1.tbl1.attr2")
                                            .AddAttribute("ds3.sc1.tbl1.attr1")
                                            .AddAttribute("ds3.sc1.tbl1.attr2"))
                .Build();
        });
    }
}
