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

            if(initialSourceTableauId is null)
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

            return Results.OnSuccess(new DeleteCommandMediation(deleteCommand, initialSourceTableauId.ParentDataSourceId));
        });

    public static Result<UpdateCommandMediation> MediateCommand(UpdateCommand updateOnMediatedDataSource, DataSource mediatedDataSource, DataSourceMediation dataSourceMediation)
        => Results.AsResult(() =>
        {
            // localize ids...

            //... of initial tableau - find the tableau from the mutation value updates
            var initialDeclaredTableau = dataSourceMediation.GetSourceAttributeId(updateOnMediatedDataSource.Mutation.ValueUpdates.Keys.First()).ParentTableauId;
            //... of selection expression

            //... of value updates
            var selectionExpression =
                updateOnMediatedDataSource.Selection
                    .Match(
                            selection => LocalizeSelectionExpression(selection.Expression, dataSourceMediation),
                            () => FALSE()
                        );

            updateCommandBuilder = updateCommandBuilder.WithSelection(conf => conf.WithExpression(selectionExpression));

            var updateCommand = updateCommandBuilder.Build();

            return Results.OnSuccess(new DeleteCommandMediation(deleteCommand, initialSourceTableauId.ParentDataSourceId));
        });

    public static Result<InsertCommandMediation> MediateCommand(InsertCommand insertOnMediatedDataSource, DataSource mediatedDataSource, DataSourceMediation dataSourceMediation, BaseCommand baseCommand)
        => Results.AsResult(() =>
        {
            // localize ids...

            //... of initial tableau

            //... of tabular data

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
