using Janus.Commons.CommandModels;
using Janus.Commons.SchemaModels;
using Janus.Commons.SelectionExpressions;
using Janus.Mask.WebApi.Lenses;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.Mask.WebApi.Translation;
public class CommandTranslation
{
    public static DeleteCommand TranslateDeleteCommand(TableauId onTableauId, string? selection = null)
    {
        var selectionExpression = TranslateSelection(selection);

        var deleteBuilder = DeleteCommandOpenBuilder.InitOpenDelete(onTableauId)
                            .WithSelection(conf => conf.WithExpression(selectionExpression));

        return deleteBuilder.Build();
    }

    public static InsertCommand TranslateInsertCommand(TableauId onTableauId, object dataTuple)
    {
        var lens = new TabularDataObjectLens<object>(); // no need for prefix, as columns require just names

        var data = new List<object> { dataTuple };

        var insertBuilder = InsertCommandOpenBuilder.InitOpenInsert(onTableauId)
                                .WithInstantiation(conf => conf.WithValues(lens.Put(data)));

        return insertBuilder.Build();
    }

    public static UpdateCommand TranslateUpdateCommand(TableauId onTableauId, object dataTuple, string? selection = null)
    {

        var lens = new TabularDataObjectLens<object>(onTableauId.ToString());
        var data = new List<object> { dataTuple };
        var mutationValues = lens.Put(data).RowData.FirstOrDefault()?.ColumnValues.ToDictionary(_ => _.Key, _ => _.Value) ?? new Dictionary<string, object?>();

        var selectionExpression = TranslateSelection(selection);

        var updateBuilder = UpdateCommandOpenBuilder.InitOpenUpdate(onTableauId)
                            .WithMutation(conf => conf.WithValues(mutationValues))
                            .WithSelection(conf => conf.WithExpression(selectionExpression));

        return updateBuilder.Build();
    }

    private static SelectionExpression TranslateSelection(string? selection)
    {
        if (string.IsNullOrWhiteSpace(selection))
        {
            return FALSE();
        }
        else
        {
            return FALSE();
        }
    }
}
