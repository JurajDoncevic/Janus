using Janus.Commons.SchemaModels;
using Janus.Mediation.SchemaMediationModels;
using Janus.Mediation.SchemaMediationModels.MediationQueryModels;
using System.ComponentModel;

namespace Janus.Mediation;

/// <summary>
/// Schema model mediation operations
/// </summary>
public static class SchemaModelMediation
{
    /// <summary>
    /// Creates a mediated data source according to the given mediation
    /// </summary>
    /// <param name="dataSourceMediation"></param>
    /// <returns>Result of a mediated data source instance</returns>
    public static Result<DataSource> MediateDataSource(DataSourceMediation dataSourceMediation)
            => Results.AsResult<DataSource>(() =>
            {
                

                var dataSourceBuilder = SchemaModelBuilder.InitDataSource(dataSourceMediation.DataSourceName)
                    .WithDescription(dataSourceMediation.DataSourceDescription)
                    .WithVersion(dataSourceMediation.DataSourceVersion);

                foreach (var schemaMediation in dataSourceMediation.SchemaMediations)
                {
                    dataSourceBuilder.AddSchema(schemaMediation.SchemaName, schemaBuilder =>
                    {
                        schemaBuilder.WithDescription(schemaMediation.SchemaDescription);

                        foreach (var tableauMediation in schemaMediation.TableauMediations)
                        {
                            schemaBuilder.AddTableau(tableauMediation.TableauName, tableauBuilder =>
                            {
                                tableauBuilder.WithDescription(tableauMediation.TableauDescription);

                                // determine nullable attrs by nullability of the FKs that brought them into the mediation
                                var nullableRefAttrs =
                                tableauMediation.SourceQuery.Joining
                                    .Match(
                                        joining => joining.Joins
                                                    .Map(join => (join, join.ForeignKeyAttributeId.NameTuple))
                                                    .Where(t => dataSourceMediation.AvailableDataSources[t.NameTuple.dataSourceName][t.NameTuple.schemaName][t.NameTuple.tableauName][t.NameTuple.attributeName].IsNullable)
                                                    .Map(t => t.join.ForeignKeyAttributeId),
                                        () => Enumerable.Empty<AttributeId>()
                                    );
                                
                                var nullableRefTabs = nullableRefAttrs.Fold(new HashSet<TableauId>(), (attrId, set) => set.Union(DetermineNullableSourceTableauIds(tableauMediation.SourceQuery.Joining.Value.Joins.ToList(), attrId)).ToHashSet());
                                 
                                foreach (var attributeMediation in tableauMediation.AttributeMediations)
                                {
                                    tableauBuilder.AddAttribute(attributeMediation.DeclaredAttributeName, attributeBuilder =>
                                    {
                                        var (dsName, scName, tblName, attrName) = attributeMediation.SourceAttributeId.NameTuple;

                                        var sourceAttribute = dataSourceMediation.AvailableDataSources[dsName][scName][tblName][attrName];

                                        bool isAttrNullable = nullableRefTabs.Contains(sourceAttribute.Tableau.Id) || sourceAttribute.IsNullable;

                                        attributeBuilder.WithDescription(dataSourceMediation.PropagateAttributeDescriptions
                                                                            ? attributeMediation.AttributeDescription ? attributeMediation.AttributeDescription.Value : sourceAttribute.Description
                                                                            : attributeMediation.AttributeDescription.Value ?? string.Empty)
                                                        .WithIsIdentity(sourceAttribute.IsIdentity) // identities are inherited by default
                                                        .WithIsNullable(isAttrNullable)
                                                        .WithDataType(sourceAttribute.DataType);

                                        return attributeBuilder;

                                    });
                                }
                                if (dataSourceMediation.PropagateUpdateSets)
                                {
                                    // get update sets - PAPER ALGORITHM
                                    foreach (var referencedTableauId in tableauMediation.SourceQuery.ReferencedTableauIds)
                                    {

                                        var (dsName, scName, tblName) = referencedTableauId.NameTuple;

                                        var updateSetsOnReferencedTableau = dataSourceMediation.AvailableDataSources[dsName][scName][tblName].UpdateSets.ToHashSet();

                                        var mediatedUpdateSets =
                                            updateSetsOnReferencedTableau
                                                .Map(us => us.AttributeIds.Intersect(tableauMediation.SourceQuery.Projection.IncludedAttributeIds))
                                                .Where(us => us.Count() > 0)
                                                .Map(us => us.Select(attrIds => tableauMediation.GetDeclaredAttributeName(attrIds)).ToHashSet());

                                        foreach (var updateSets in mediatedUpdateSets)
                                        {
                                            tableauBuilder.AddUpdateSet(conf => conf.WithAttributesNamed(updateSets!));
                                        }

                                    }
                                }


                                return tableauBuilder;
                            });
                        }

                        return schemaBuilder;
                    });
                }
                return dataSourceBuilder.Build();
            });

    private static HashSet<TableauId> DetermineNullableSourceTableauIds(List<Join> joins, AttributeId startingNullableFkAttribute)
    {
        var nullableTableauIds = 
            joins.Where(join => join.ForeignKeyAttributeId.Equals(startingNullableFkAttribute))
                 .Map(join => join.PrimaryKeyTableauId)
                 .ToHashSet();

        var primaryKeysOnNullableTableaus =
            joins.Where(join => nullableTableauIds.Contains(join.ForeignKeyTableauId))
                 .Map(join => join.PrimaryKeyAttributeId);
        nullableTableauIds = nullableTableauIds.Union(primaryKeysOnNullableTableaus.Map(pk => pk.ParentTableauId)).ToHashSet();

        foreach (var pkOnNullTab in primaryKeysOnNullableTableaus)
        {
            nullableTableauIds = nullableTableauIds.Union(DetermineNullableSourceTableauIds(joins, pkOnNullTab)).ToHashSet();
        }

        return nullableTableauIds;
    }
}
