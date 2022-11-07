using Janus.Commons;
using Janus.Commons.SchemaModels;
using Janus.Mediation.MediationModels;

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
            => ResultExtensions.AsResult<DataSource>(() =>
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

                                foreach (var attributeMediation in tableauMediation.AttributeMediations)
                                {
                                    tableauBuilder.AddAttribute(attributeMediation.DeclaredAttributeName, attributeBuilder =>
                                    {
                                        var (dsName, scName, tblName, attrName) = Utils.GetNamesFromAttributeId(attributeMediation.SourceAttributeId);

                                        var sourceAttribute = dataSourceMediation.AvailableDataSources[dsName][scName][tblName][attrName];

                                        attributeBuilder.WithDescription(dataSourceMediation.PropagateAttributeDescriptions
                                                                            ? attributeMediation.AttributeDescription ? attributeMediation.AttributeDescription.Value : sourceAttribute.Description
                                                                            : attributeMediation.AttributeDescription.Value ?? string.Empty)
                                                        .WithIsIdentity(sourceAttribute.IsIdentity) // identities are inherited by default
                                                        .WithIsNullable(sourceAttribute.IsNullable)
                                                        .WithDataType(sourceAttribute.DataType);

                                        return attributeBuilder;

                                    });
                                }
                                if (dataSourceMediation.PropagateUpdateSets)
                                {
                                    // get update sets - PAPER ALGORITHM
                                    foreach (var referencedTableauId in tableauMediation.SourceQuery.ReferencedTableauIds)
                                    {

                                        var (dsName, scName, tblName) = Utils.GetNamesFromTableauId(referencedTableauId);

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
}
