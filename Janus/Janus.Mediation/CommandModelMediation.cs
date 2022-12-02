using Janus.Commons.CommandModels;
using Janus.Commons.SchemaModels;
using Janus.Commons.SelectionExpressions;
using static Janus.Commons.SelectionExpressions.Expressions;
using Janus.Mediation.CommandMediationModels;
using Janus.Mediation.SchemaMediationModels;
using Janus.Commons.QueryModels;
using Janus.Commons.DataModels;

namespace Janus.Mediation;
/// <summary>
/// Command model mediation operations
/// </summary>
public class CommandModelMediation
{
    public static Result<DeleteCommandMediation> MediateCommand(DeleteCommand deleteOnMediatedDataSource, DataSource mediatedDataSource, DataSourceMediation dataSourceMediation)
        => Results.AsResult(() =>
        {
            //localize ids of initial tableau
            var initialSourceTableauId =
                dataSourceMediation.GetSourceInitialTableauId(deleteOnMediatedDataSource.OnTableauId);

            if (initialSourceTableauId is null)
            {
                return Results.OnFailure<DeleteCommandMediation>("Couldn't find the initial tableau from the tableau mediation source query");
            }


            //localize ids of selection expression
            var selectionExpression =
                deleteOnMediatedDataSource.Selection
                    .Match(
                            selection => LocalizeSelectionExpression(selection.Expression, dataSourceMediation),
                            () => FALSE()
                        );

            // build the command
            var deleteCommand = 
                DeleteCommandOpenBuilder.InitOpenDelete(initialSourceTableauId)
                    .WithSelection(conf => conf.WithExpression(selectionExpression))
                    .Build();

            // prevalidate command before sending?

            return Results.OnSuccess(new DeleteCommandMediation(deleteCommand, initialSourceTableauId.ParentDataSourceId, deleteOnMediatedDataSource, dataSourceMediation));
        });

    public static Result<UpdateCommandMediation> MediateCommand(UpdateCommand updateOnMediatedDataSource, DataSource mediatedDataSource, DataSourceMediation dataSourceMediation)
        => Results.AsResult(() =>
        {
            //localize id of initial tableau - find the tableau from the mutation value updates
            // this is because only an update set can be updated
            var mutationAttrNames = updateOnMediatedDataSource.Mutation.ValueUpdates.Keys;
            var mutationAttrMapping = mutationAttrNames.ToDictionary(attrName => attrName, attrName => dataSourceMediation.GetSourceAttributeId(AttributeId.From(updateOnMediatedDataSource.OnTableauId, attrName))); // dict of declared attr name and source attr id
            var initialSourceTableauId = dataSourceMediation.GetSourceAttributeId(mutationAttrMapping.Values.First()!)!.ParentTableauId;

            //localize attribute names in value updates
            var localizedValueUpdates =
                updateOnMediatedDataSource
                    .Mutation
                    .ValueUpdates
                    .ToDictionary(vu => mutationAttrMapping[vu.Key]!.AttributeName, vu => vu.Value);

            //localize attribute ids of selection expression
            var localizedSelectionExpression =
                updateOnMediatedDataSource.Selection
                    .Match(
                            selection => LocalizeSelectionExpression(selection.Expression, dataSourceMediation),
                            () => FALSE()
                        );

            // build the command
            var updateCommand =
                UpdateCommandOpenBuilder.InitOpenUpdate(initialSourceTableauId)
                    .WithMutation(conf => conf.WithValues(localizedValueUpdates))
                    .WithSelection(conf => conf.WithExpression(localizedSelectionExpression))
                    .Build();

            // Prevalidate command before sending?

            return Results.OnSuccess(new UpdateCommandMediation(updateCommand, initialSourceTableauId.ParentDataSourceId, updateOnMediatedDataSource, dataSourceMediation));
        });

    public static Result<InsertCommandMediation> MediateCommand(InsertCommand insertOnMediatedDataSource, DataSource mediatedDataSource, DataSourceMediation dataSourceMediation, BaseCommand baseCommand)
        => Results.AsResult(() =>
        {
            //localize attribute id of initial tableau
            var initialSourceTableauId =
                dataSourceMediation.GetSourceInitialTableauId(insertOnMediatedDataSource.OnTableauId)!;

            //localize column names of tabular data
            var columnNameMapping = // declared column name, source attr id
                insertOnMediatedDataSource.Instantiation.TabularData.ColumnNames
                .ToDictionary(declAttrName => declAttrName, declAttrName => dataSourceMediation.GetDeclaredAttributeId(AttributeId.From(insertOnMediatedDataSource.OnTableauId, declAttrName)));

            var localizedColumnDataTypes = // localize attribute names and their data types
                insertOnMediatedDataSource.Instantiation.TabularData.ColumnDataTypes
                .ToDictionary(cdt => columnNameMapping[cdt.Key]!.AttributeName, cdt => cdt.Value);

            var localizedTabularData =  // build the tabular data with localized column names
                insertOnMediatedDataSource.Instantiation.TabularData.RowData.Fold(
                    TabularDataBuilder.InitTabularData(localizedColumnDataTypes),
                    (rowData, builder) => builder.AddRow(
                        conf => conf.WithRowData(rowData.ColumnValues.ToDictionary(cv => columnNameMapping[cv.Key]!.AttributeName, cv => cv.Value))
                        )
                    ).Build();

            // build command
            var insertCommand =
                InsertCommandOpenBuilder.InitOpenInsert(initialSourceTableauId)
                    .WithInstantiation(conf => conf.WithValues(localizedTabularData))
                    .Build();

            // prevalidate command before sending?

            return Results.OnSuccess(new InsertCommandMediation(insertCommand, initialSourceTableauId.ParentDataSourceId, insertOnMediatedDataSource, dataSourceMediation));
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
}
