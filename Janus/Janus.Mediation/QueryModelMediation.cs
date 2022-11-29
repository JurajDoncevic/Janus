using Janus.Commons;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Commons.SelectionExpressions;
using Janus.Mediation.QueryMediationModels;
using Janus.Mediation.SchemaMediationModels;
using static Janus.Commons.SelectionExpressions.Expressions;
namespace Janus.Mediation;
/// <summary>
/// Query model mediation operations
/// </summary>
public static class QueryModelMediation
{
    public static Result<QueryMediation> MediateQuery(DataSource mediatedDataSource, DataSourceMediation dataSourceMediation, Query query)
        => Results.AsResult(() =>
        {
            // initial source tableau id
            var initialSourceTableauId =
                Utils.GetNamesFromTableauId(query.OnTableauId).Identity()
                .Map(names => dataSourceMediation[names.schemaName]![names.tableauName]!.SourceQuery.InitialTableauId)
                .Data;

            // translate projected attrs to source attr ids
            var projectedSourceAttributeIds = query.Projection.Match(
                projection => projection.IncludedAttributeIds
                                        .Map(declaredAttrId => dataSourceMediation.GetSourceAttributeId(declaredAttrId)),
                () => new List<string>()
                );

            // translate explicit joins between mediated tableaus; expand joins inside mediated tableaus
            var joinsFromQuery = query.Joining.Match(
                joining => joining.Joins
                                  .Map(join => (fkAttrId: dataSourceMediation.GetSourceAttributeId(join.ForeignKeyAttributeId),
                                                pkAttrId: dataSourceMediation.GetSourceAttributeId(join.PrimaryKeyAttributeId))),
                () => new List<(string? fkAttrId, string? pkAttrId)>()
                )
                .Map(j => Join.CreateJoin(j.fkAttrId, j.pkAttrId));

            var joinsInsideMediatedTableau =
                query.Joining.Match(
                    joining => joining.Joins
                                      .Map(j => new List<string> { j.ForeignKeyTableauId, j.PrimaryKeyTableauId })
                                      .SelectMany(tableauIds => tableauIds)
                                      .Map(tableauId => Utils.GetNamesFromTableauId(tableauId))
                                      .Map(names => dataSourceMediation[names.schemaName]![names.tableauName]!.SourceQuery.Joining)
                                      .Map(joining => joining.Value.Joins)
                                      .Map(joins => joins.Map(join => (fkAttrId: join.ForeignKeyAttributeId, pkAttrId: join.PrimaryKeyAttributeId)))
                                      .SelectMany(t => t),
                    () => Enumerable.Empty<(string fkAttrId, string pkAttrId)>()
                    )
                .Map(j => Join.CreateJoin(j.fkAttrId, j.pkAttrId));

            var localizedSelectionExpression = query.Selection.Match(
                selection => LocalizeSelectionExpression(selection.Expression, dataSourceMediation),
                () => FALSE()
                );

            // determine split queries - for each join set from the same data source - split a colored graph

            // split projection into separate queries - keep the joining attributes

            // if conjunctive selection split selection into separate queries, else put into finalizing selection

            return Results.OnException<QueryMediation>(new NotImplementedException());
        });

    public static Result<TabularData> MediateQueryResults(DataSource mediatedDataSource, DataSourceMediation dataSourceMediation, QueryMediation queryMediation)
        => Results.AsResult(() =>
        {

            return Results.OnException<TabularData>(new NotImplementedException());
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

    private static bool IsConjunctiveExpression(SelectionExpression selectionExpression)
    {
        return selectionExpression switch
        {
            OrOperator => false,
            NotOperator => false,
            AndOperator and => IsConjunctiveExpression(and.LeftOperand) && IsConjunctiveExpression(and.RightOperand),
            _ => true // literals and comparison ops can't have logical operations inside of them
        };
    }

    private class QueryPartition
    {
        public string InitialTableau { get; set; }
        public IList<Join> Joins { get; set; }

        public bool ContainsTableau(string tableauId)
        {
            return Joins.Any(j => j.ForeignKeyTableauId == tableauId || j.PrimaryKeyTableauId == tableauId);
        }

        public override bool Equals(object? obj)
        {
            return obj is QueryPartition other &&
                   InitialTableau == other.InitialTableau &&
                   Joins.SequenceEqual(other.Joins);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(InitialTableau, Joins);
        }
    }

    private static bool AreFromSameDataSource(string tableauId1, string tableauId2)
    {
        return Utils.GetNamesFromTableauId(tableauId1).dataSourceName
                .Equals(Utils.GetNamesFromTableauId(tableauId2).dataSourceName);
    }

    private static (IEnumerable<QueryPartition> queryPartitions, IEnumerable<Join> globalJoins) DetermineQueryPartitions(string initialTableauId, List<Join> joins)
    {
        var queryPartitions = new List<QueryPartition>
        {
            new QueryPartition()
            {
                InitialTableau = initialTableauId,
                Joins = new List<Join>()
            }
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
                    ?? new QueryPartition()
                    {
                        InitialTableau = join.ForeignKeyTableauId,
                        Joins = new List<Join>()
                    };
                // whatever the case, add the query join to the selected query partition
                targetQueryPartition.Joins.Add(join);
            }
            else // 
            {
                globalJoins.Add(join);
            }
        }

        return(queryPartitions, globalJoins);
    }
}
