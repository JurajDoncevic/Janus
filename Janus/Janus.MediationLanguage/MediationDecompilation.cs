using Janus.Mediation.SchemaMediationModels;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.MediationLanguage;
public static class MediationDecompilation
{
    public static string ToMediationScript(this DataSourceMediation dataSourceMediation)
    {
        var dataSourceMediationClause =
            $"DATASOURCE {dataSourceMediation.DataSourceName} VERSION \"{dataSourceMediation.DataSourceVersion}\" #{dataSourceMediation.DataSourceDescription}#\n";
        
        IEnumerable<string> schemaMediationClauses = Enumerable.Empty<string>();
        foreach (var schemaMediation in dataSourceMediation.SchemaMediations)
        {
            var schemaMediationClause = $"WITH SCHEMA {schemaMediation.SchemaName} #{schemaMediation.SchemaDescription}#\n";

            IEnumerable<string> tableauMediationClauses = Enumerable.Empty<string>();
            foreach (var tableauMediation in schemaMediation.TableauMediations)
            {
                var tableauMediationClause = $"WITH TABLEAU {tableauMediation.TableauName} #{tableauMediation.TableauDescription}#\n";

                var attributeDeclarationClause = 
                    $"WITH ATTRIBUTES\n\t{string.Join(",\n\t", tableauMediation.AttributeMediations.Map(attrMed => $"{attrMed.DeclaredAttributeName}" + (attrMed.AttributeDescription ? $" #{attrMed.AttributeDescription.Value}#" : string.Empty)))}";

                var beingKeyword = "BEING";

                var sourceQueryProjectionClause =
                    $"SELECT {string.Join(", ", tableauMediation.SourceQuery.Projection.IncludedAttributeIds)}";

                var sourceQueryFromClause = $"FROM {tableauMediation.SourceQuery.InitialTableauId}";

                IEnumerable<string> joinClauses =
                    tableauMediation.SourceQuery.Joining
                    ? tableauMediation.SourceQuery.Joining.Value.Joins.Map(join => $"JOIN {join.PrimaryKeyTableauId} ON {join.ForeignKeyAttributeId} == {join.PrimaryKeyAttributeId}")
                    : Enumerable.Empty<string>();

                var sourceQueryClause = $"\t{sourceQueryProjectionClause}\n{sourceQueryFromClause}{(joinClauses.Count() > 0 ? "\n" : string.Empty)}{string.Join("\n", joinClauses)}".Replace("\n", "\n\t");

                tableauMediationClause +=
                    $"\t{attributeDeclarationClause}\n{beingKeyword}\n{sourceQueryClause}".Replace("\n", "\n\t");

                tableauMediationClauses = tableauMediationClauses.Append(tableauMediationClause);
            }
            schemaMediationClause += "\t" + string.Join("\n", tableauMediationClauses).Replace("\n", "\n\t");

            schemaMediationClauses = schemaMediationClauses.Append(schemaMediationClause);
        }

        dataSourceMediationClause += "\t" + string.Join("\n", schemaMediationClauses).Replace("\n", "\n\t");

        var propUpdateSetsSetting = "PROPAGATE UPDATE SETS";
        var propAttrDescriptionsSetting = "PROPAGATE ATTRIBUTE DESCRIPTIONS";

        var settings = 
            (dataSourceMediation.PropagateUpdateSets
                ? "\t" + propUpdateSetsSetting + "\n"
                : string.Empty)
            +
            (dataSourceMediation.PropagateAttributeDescriptions
                ? "\t" + propAttrDescriptionsSetting + "\n"
                : string.Empty);

        var settingsClause = !string.IsNullOrWhiteSpace(settings) ? "SETTING\n" + settings : string.Empty;

        dataSourceMediationClause = $"{settingsClause}{dataSourceMediationClause}";

        return dataSourceMediationClause;
    }
}
