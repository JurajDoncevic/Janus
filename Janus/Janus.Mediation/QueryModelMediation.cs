using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Commons.SchemaModels.Building;
using Janus.Commons.SelectionExpressions;
using Janus.Mediation.QueryMediationModels;
using Janus.Mediation.SchemaMediationModels;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.Mediation;

/// <summary>
/// Query model mediation operations
/// </summary>
public static partial class QueryModelMediation
{
    /// <summary>
    /// Creates a query mediation for a query on a mediated data source
    /// </summary>
    /// <param name="mediatedDataSource">Mediated data source</param>
    /// <param name="dataSourceMediation">Data source schema model mediation</param>
    /// <param name="queryOnMediatedDataSource">Query on the mediated data source</param>
    /// <returns>Query mediation containing partitioned and localized queries, together with global join, selection, and projection instructions</returns>
    public static Result<QueryMediation> MediateQuery(DataSource mediatedDataSource, DataSourceMediation dataSourceMediation, Query queryOnMediatedDataSource)
        => Results.AsResult(() =>
        {
            // initial source tableau id
            var initialSourceTableauId =
                queryOnMediatedDataSource.OnTableauId.NameTuple.Identity()
                .Map(names => dataSourceMediation[names.schemaName]![names.tableauName]!.SourceQuery.InitialTableauId)
                .Data;

            // translate projected attrs to source attr ids
            var projectedSourceAttributeIds = queryOnMediatedDataSource.Projection.Match(
                projection => projection.IncludedAttributeIds
                                        .Map(declaredAttrId => dataSourceMediation.GetSourceAttributeId(declaredAttrId)!),
                () => new List<AttributeId>()
                );

            // translate explicit joins between mediated tableaus... 
            var joinsFromQuery = queryOnMediatedDataSource.Joining.Match(
                joining => joining.Joins
                                  .Map(join => (fkAttrId: dataSourceMediation.GetSourceAttributeId(join.ForeignKeyAttributeId),
                                                pkAttrId: dataSourceMediation.GetSourceAttributeId(join.PrimaryKeyAttributeId))),
                () => new List<(AttributeId? fkAttrId, AttributeId? pkAttrId)>()
                )
                .Map(j => Join.CreateJoin(j.fkAttrId!, j.pkAttrId!));
            // ...expand joins inside mediated tableaus
            var joinsInsideMediatedTableau =
                queryOnMediatedDataSource.Joining.Match(
                    joining => joining.Joins
                                      .Map(j => new List<TableauId> { j.ForeignKeyTableauId, j.PrimaryKeyTableauId })
                                      .SelectMany(tableauIds => tableauIds)
                                      .Append(initialSourceTableauId)
                                      .Map(tableauId => tableauId.NameTuple)
                                      .Map(names => dataSourceMediation[names.schemaName]![names.tableauName]!.SourceQuery.Joining)
                                      .Map(joining => joining.Value.Joins)
                                      .Map(joins => joins.Map(join => (fkAttrId: join.ForeignKeyAttributeId, pkAttrId: join.PrimaryKeyAttributeId)))
                                      .SelectMany(t => t),
                    () => initialSourceTableauId.Identity()
                            .Map(tableauId => tableauId.NameTuple)
                            .Map(names => dataSourceMediation[names.schemaName]![names.tableauName]!.SourceQuery.Joining)
                            .Data
                            .Map(joining => joining.Joins)
                            .Map(joins => joins.Map(join => (fkAttrId: join.ForeignKeyAttributeId, pkAttrId: join.PrimaryKeyAttributeId)))
                            .Match(
                                joins => joins,
                                () => Enumerable.Empty<(AttributeId fkAttrId, AttributeId pkAttrId)>()
                            )
                    )
                .Map(j => Join.CreateJoin(j.fkAttrId, j.pkAttrId));

            // localize selection
            var localizedSelectionExpression = queryOnMediatedDataSource.Selection.Match(
                selection => LocalizeSelectionExpression(selection.Expression, dataSourceMediation),
                () => TRUE()
                );

            // determine query partitions - for each join set from the same data source - split a colored graph
            var (queryPartitions, globalJoins) = DetermineQueryPartitions(initialSourceTableauId, joinsFromQuery.Union(joinsInsideMediatedTableau));

            // split projection into separate queries - keep the joining pk and fk attributes from global joins
            queryPartitions =
                queryPartitions
                    .Map(queryPartition =>
                    {
                        // projection attrs
                        queryPartition.AddProjectionAttributes(
                            projectedSourceAttributeIds
                                .Where(attrId => queryPartition.IsParentTableauReferenced(attrId))
                                .ToHashSet());
                        // join attrs from global joins
                        queryPartition.AddProjectionAttributes(
                            globalJoins.SelectMany(j => new HashSet<AttributeId>() { j.ForeignKeyAttributeId, j.PrimaryKeyAttributeId })
                                                .ToHashSet());

                        return queryPartition;
                    });

            // keep selection expression attributes in projection
            var attributeIdsInSelection = GetAttributeIdsFromSelection(localizedSelectionExpression);
            queryPartitions =
                        queryPartitions.Map(
                            queryPartition =>
                            {
                                queryPartition.AddProjectionAttributes(attributeIdsInSelection.Where(attrId => queryPartition.IsParentTableauReferenced(attrId)));
                                return queryPartition;
                            });


            // if conjunctive selection, split selection into query partitions, else put into finalizing selection
            SelectionExpression finalizingSelectionExpression = TRUE();
            if (IsConjunctiveExpression(localizedSelectionExpression))
            {
                var (comparisons, literals) = GetComparisonsAndLiterals(localizedSelectionExpression);

                queryPartitions =
                    queryPartitions.Map(
                        queryPartition =>
                        {
                            // once more add the attributes from the selection into the projection
                            queryPartition.AddProjectionAttributes(
                                comparisons.Map(c => c.AttributeId).Where(attrId => queryPartition.IsParentTableauReferenced(attrId))
                                );

                            // construct a conjunctive partitioned selection expression - literals remain, let the sources handle them :)
                            queryPartition.SelectionExpression =
                                literals.Count() == 1 && comparisons.Count() == 0 // if there is just one literal and no comparisons
                                ? queryPartition.SelectionExpression // just pass the TRUE or FALSE
                                : literals.Fold((SelectionExpression)TRUE(),
                                    (lit, selExpr) =>
                                        AND(lit,
                                            comparisons.Fold(selExpr,
                                                (comp, exp) => AND(comp, exp))
                                            )
                                );
                            return queryPartition;
                        });
            }
            else
            {
                finalizingSelectionExpression = localizedSelectionExpression;
            }

            // create finalizing projection
            var finalizingProjection = projectedSourceAttributeIds;

            // build queries and query mediation
            var partitionedQueries =
                queryPartitions.Map(
                    qp => QueryModelOpenBuilder.InitOpenQuery(qp.InitialTableauId)
                            .WithJoining(configuration => qp.Joins.Fold(configuration, (join, conf) => conf.AddJoin(join.ForeignKeyAttributeId, join.PrimaryKeyAttributeId)))
                            .WithSelection(configuration => configuration.WithExpression(qp.SelectionExpression))
                            .WithProjection(configuration => qp.ProjectionAttributeIds.Fold(configuration, (attrId, conf) => conf.AddAttribute(attrId)))
                            .Build());

            QueryMediation queryMediation =
                new QueryMediation(
                    queryOnMediatedDataSource,
                    partitionedQueries,
                    globalJoins,
                    finalizingSelectionExpression,
                    finalizingProjection,
                    dataSourceMediation);

            return Results.OnSuccess(queryMediation);
        });

    public static Result<TabularData> MediateQueryResults(QueryMediation queryMediation, IEnumerable<TabularData> queryResults)
        => Results.AsResult(() =>
        {
            if (queryResults.Count() < 1)
            {
                return Results.OnFailure<TabularData>("No query results given");
            }

            // execute global joins
            var joinResult =
                queryMediation.FinalizingJoins.Fold(Results.OnSuccess(queryResults),
                    (join, currentResult) =>
                        currentResult.Bind(currTabs =>
                        {
                            var fkResult = currTabs.First(r => r.ContainsColumn(join.ForeignKeyAttributeId.ToString()));
                            var pkResult = currTabs.First(r => r.ContainsColumn(join.PrimaryKeyAttributeId.ToString()));

                            var currentJoinResult =
                                TabularDataOperations.EquiJoinTabularData(fkResult, pkResult, join.ForeignKeyAttributeId.ToString(), join.PrimaryKeyAttributeId.ToString())
                                .Map(r => currTabs.Except(new[] { fkResult, pkResult }).Append(r));

                            return currentJoinResult;
                        })).Map(rs => rs.Single()); // throws exception if there is no single tabular result of joins

            // execute finalizing selection
            var selectionResult = joinResult.Bind(joinedTabular => TabularDataOperations.SelectRowData(joinedTabular, queryMediation.FinalizingSelection));

            // execute finalizing projection
            var projectionResult = selectionResult.Bind(selectedTabular => TabularDataOperations.ProjectColumns(selectedTabular, queryMediation.FinalizingProjection.Map(attrId => attrId.ToString()).ToHashSet()));

            // globalize column names
            var globalizedNamesResult = projectionResult.Bind(projectedData =>
            {
                var nameMappingDict = projectedData.ColumnNames.ToDictionary(cn => cn, cn => queryMediation.DataSourceMediation.GetDeclaredAttributeId(AttributeId.From(cn)));
                if (nameMappingDict.Values.Contains(null))
                {
                    return Results.OnFailure<TabularData>($"Couldn't globalize column names: {string.Join(", ", nameMappingDict.Where(_ => _.Value is null).Map(kv => kv.Key))}");
                }

                var columnDataTypes = projectedData.ColumnDataTypes.ToDictionary(kv => nameMappingDict[kv.Key]!.ToString(), kv => kv.Value!);

                var tabularDataBuilder = TabularDataBuilder.InitTabularData(columnDataTypes!)
                                                           .WithName(projectedData.Name);

                foreach (var rowData in projectedData.RowData)
                {
                    var columnValues = rowData.ColumnValues.ToDictionary(kv => nameMappingDict[kv.Key]!.ToString(), kv => kv.Value);
                    tabularDataBuilder = tabularDataBuilder.AddRow(conf => conf.WithRowData(columnValues));
                }

                return Results.OnSuccess(tabularDataBuilder.Build());
            });

            return globalizedNamesResult;
        });


    private static SelectionExpression LocalizeSelectionExpression(SelectionExpression selectionExpression, DataSourceMediation mediation)
    {
        return selectionExpression switch
        {
            AndOperator and => AND(LocalizeSelectionExpression(and.LeftOperand, mediation), LocalizeSelectionExpression(and.RightOperand, mediation)),
            OrOperator or => OR(LocalizeSelectionExpression(or.LeftOperand, mediation), LocalizeSelectionExpression(or.RightOperand, mediation)),
            NotOperator not => NOT(LocalizeSelectionExpression(not.Operand, mediation)),
            GreaterThan gt => GT(mediation.GetSourceAttributeId(gt.AttributeId)!, gt.Value),
            GreaterOrEqualThan ge => GE(mediation.GetSourceAttributeId(ge.AttributeId)!, ge.Value),
            LesserThan lt => LT(mediation.GetSourceAttributeId(lt.AttributeId)!, lt.Value),
            LesserOrEqualThan le => LE(mediation.GetSourceAttributeId(le.AttributeId)!, le.Value),
            EqualAs eq => EQ(mediation.GetSourceAttributeId(eq.AttributeId)!, eq.Value),
            NotEqualAs neq => NEQ(mediation.GetSourceAttributeId(neq.AttributeId)!, neq.Value),
            Literal literal => literal
        };
    }

    private static HashSet<AttributeId> GetAttributeIdsFromSelection(SelectionExpression selectionExpression)
        => selectionExpression switch
        {
            ComparisonOperator comparison => new HashSet<AttributeId>() { comparison.AttributeId },
            LogicalBinaryOperator logicalOperator => new HashSet<AttributeId>()
                                                        .Union(GetAttributeIdsFromSelection(logicalOperator.LeftOperand))
                                                        .Union(GetAttributeIdsFromSelection(logicalOperator.RightOperand))
                                                        .ToHashSet(),
            LogicalUnaryOperator logicalOperator => new HashSet<AttributeId>()
                                                        .Union(GetAttributeIdsFromSelection(logicalOperator.Operand))
                                                        .ToHashSet(),
            _ => new HashSet<AttributeId>()
        };

    private static (IEnumerable<ComparisonOperator> comparisons, IEnumerable<Literal> literals) GetComparisonsAndLiterals(SelectionExpression selectionExpression)
    {
        IEnumerable<ComparisonOperator> comparisons = Enumerable.Empty<ComparisonOperator>();
        IEnumerable<Literal> literals = Enumerable.Empty<Literal>();

        (comparisons, literals) =
        selectionExpression switch
        {
            ComparisonOperator comparison => (comparisons.Append(comparison), literals),
            Literal literal => (comparisons, literals.Append(literal)),
            LogicalUnaryOperator logicalOperator => GetComparisonsAndLiterals(logicalOperator.Operand),
            LogicalBinaryOperator logicalOperator => (left: GetComparisonsAndLiterals(logicalOperator.LeftOperand), right: GetComparisonsAndLiterals(logicalOperator.RightOperand))
                                                        .Identity()
                                                        .Map(t => (t.left.comparisons.Union(t.right.comparisons), t.left.literals.Union(t.right.literals)))
                                                        .Data,
            _ => (comparisons, literals)
        };

        return (comparisons, literals);
    }

    private static bool IsConjunctiveExpression(SelectionExpression selectionExpression)
    {
        return selectionExpression switch
        {
            OrOperator => false,
            NotOperator => false,
            AndOperator and => IsConjunctiveExpression(and.LeftOperand) && IsConjunctiveExpression(and.RightOperand),
            _ => false // literals and comparison ops can't have logical operations inside of them
        };
    }

    private static bool AreFromSameDataSource(TableauId tableauId1, TableauId tableauId2)
    {
        return tableauId1.DataSourceName.Equals(tableauId2.DataSourceName);
    }

    private static (IEnumerable<QueryPartition> queryPartitions, IEnumerable<Join> globalJoins) DetermineQueryPartitions(TableauId initialTableauId, IEnumerable<Join> joins)
    {
        var queryPartitions = new List<QueryPartition>
        {
            new QueryPartition(initialTableauId)
        };
        var globalJoins = new List<Join>();

        foreach (var join in joins)
        {
            // if this join can be run on the datasource
            if (AreFromSameDataSource(join.ForeignKeyTableauId, join.PrimaryKeyTableauId))
            {
                // either find an existing query partition with referenced tableaus
                // or make a new one
                QueryPartition targetQueryPartition =
                    queryPartitions.SingleOrDefault(qj => qj.ContainsTableau(join.PrimaryKeyTableauId))
                    ?? queryPartitions.SingleOrDefault(qj => qj.ContainsTableau(join.ForeignKeyTableauId))
                    ?? new QueryPartition(join.ForeignKeyTableauId);
                // whatever the case, add the query join to the selected query partition
                targetQueryPartition.AddJoin(join);
            }
            else // 
            {
                globalJoins.Add(join);
            }
        }

        return (queryPartitions, globalJoins);
    }
}
