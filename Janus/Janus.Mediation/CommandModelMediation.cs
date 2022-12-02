using Janus.Commons.CommandModels;
using Janus.Commons.SchemaModels;
using Janus.Commons.SelectionExpressions;
using static Janus.Commons.SelectionExpressions.Expressions;
using Janus.Mediation.CommandMediationModels;
using Janus.Mediation.SchemaMediationModels;
using Janus.Commons.QueryModels;

namespace Janus.Mediation;
/// <summary>
/// Command model mediation operations
/// </summary>
public class CommandModelMediation
{
    public static Result<DeleteCommandMediation> MediateCommand(DeleteCommand deleteOnMediatedDataSource, DataSource mediatedDataSource, DataSourceMediation dataSourceMediation)
        => Results.AsResult(() =>
        {

            // localize ids...

            //... of initial tableau
            var initialSourceTableauId =
                dataSourceMediation.GetSourceInitialTableauId(deleteOnMediatedDataSource.OnTableauId);

            if (initialSourceTableauId is null)
            {
                return Results.OnFailure<DeleteCommandMediation>("Couldn't find the initial tableau from the tableau mediation source query");
            }

            var deleteCommandBuilder = DeleteCommandOpenBuilder.InitOpenDelete(initialSourceTableauId);

            //... of selection expression
            var selectionExpression =
                deleteOnMediatedDataSource.Selection
                    .Match(
                            selection => LocalizeSelectionExpression(selection.Expression, dataSourceMediation),
                            () => FALSE()
                        );
            deleteCommandBuilder = deleteCommandBuilder.WithSelection(conf => conf.WithExpression(selectionExpression));

            var deleteCommand = deleteCommandBuilder.Build();

            // prevalidate command before sending?

            return Results.OnSuccess(new DeleteCommandMediation(deleteCommand, initialSourceTableauId.ParentDataSourceId));
        });

    public static Result<UpdateCommandMediation> MediateCommand(UpdateCommand updateOnMediatedDataSource, DataSource mediatedDataSource, DataSourceMediation dataSourceMediation)
        => Results.AsResult(() =>
        {
            //localize id of initial tableau - find the tableau from the mutation value updates
            var mutationAttrNames = updateOnMediatedDataSource.Mutation.ValueUpdates.Keys;
            var mutationAttrMapping = mutationAttrNames.ToDictionary(attrName => attrName, attrName => dataSourceMediation.GetSourceAttributeId(AttributeId.From(updateOnMediatedDataSource.OnTableauId, attrName))); // dict of declared attr name and source attr id
            var initialSourceTableauId = dataSourceMediation.GetSourceAttributeId(mutationAttrMapping.Values.First()!)!.ParentTableauId;

            //localize ids of value updates
            var localizedValueUpdates =
                updateOnMediatedDataSource
                    .Mutation
                    .ValueUpdates
                    .ToDictionary(vu => mutationAttrMapping[vu.Key]!.AttributeName, vu => vu.Value);

            //localize ids of selection expression
            var localizedSelectionExpression =
                updateOnMediatedDataSource.Selection
                    .Match(
                            selection => LocalizeSelectionExpression(selection.Expression, dataSourceMediation),
                            () => FALSE()
                        );

            var updateCommand =
                UpdateCommandOpenBuilder.InitOpenUpdate(initialSourceTableauId)
                    .WithMutation(conf => conf.WithValues(localizedValueUpdates))
                    .WithSelection(conf => conf.WithExpression(localizedSelectionExpression))
                    .Build();

            // Prevalidate command before sending?

            return Results.OnSuccess(new UpdateCommandMediation(updateCommand, initialSourceTableauId.ParentDataSourceId));
        });

    public static Result<InsertCommandMediation> MediateCommand(InsertCommand insertOnMediatedDataSource, DataSource mediatedDataSource, DataSourceMediation dataSourceMediation, BaseCommand baseCommand)
        => Results.AsResult(() =>
        {
            // localize ids...

            //... of initial tableau

            //... of tabular data

            // prevalidate command before sending?

            return Results.OnException<InsertCommandMediation>(new NotImplementedException());
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
