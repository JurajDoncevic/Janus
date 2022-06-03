using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Janus.Commons.SelectionExpressions.SelectionExpressions;

namespace Janus.Commons.CommandModels;

public class DeleteCommandOpenBuilder
{
    private readonly string _onTableauId;
    private Option<CommandSelection> _selection;
    internal DeleteCommandOpenBuilder(string onTableauId)
    {
        _onTableauId = onTableauId;
        _selection = Option<CommandSelection>.None;
    }

    public DeleteCommand Build()
        => new DeleteCommand(_onTableauId, _selection);

    public static DeleteCommandOpenBuilder InitOpenDelete(string onTableauId)
    {
        return new DeleteCommandOpenBuilder(onTableauId);
    }

    public DeleteCommandOpenBuilder WithSelection(Func<CommandSelectionOpenBuilder, CommandSelectionOpenBuilder> configuration)
    {
        var selectionBuilder = new CommandSelectionOpenBuilder();
        _selection = Option<CommandSelection>.Some(configuration(selectionBuilder).Build());

        return this;
    }

}
