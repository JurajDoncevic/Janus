﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.Commons.CommandModels;

/// <summary>
/// Builder class to internally construct a delete command without validation on a data source
/// </summary>
internal class DeleteCommandOpenBuilder
{
    private readonly string _onTableauId;
    private Option<CommandSelection> _selection;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="onTableauId">Command's starting tableau</param>
    private DeleteCommandOpenBuilder(string onTableauId)
    {
        _onTableauId = onTableauId;
        _selection = Option<CommandSelection>.None;
    }

    /// <summary>
    /// Builds the specified delete command
    /// </summary>
    /// <returns></returns>
    internal DeleteCommand Build()
        => new DeleteCommand(_onTableauId, _selection);

    /// <summary>
    /// Initializes the delete command open builder
    /// </summary>
    /// <param name="onTableauId">Command's starting tableau</param>
    /// <returns></returns>
    internal static DeleteCommandOpenBuilder InitOpenDelete(string onTableauId)
    {
        return new DeleteCommandOpenBuilder(onTableauId);
    }

    /// <summary>
    /// Sets the delete command's selection clause
    /// </summary>
    /// <param name="configuration">Selection configuration</param>
    /// <returns></returns>
    internal DeleteCommandOpenBuilder WithSelection(Func<CommandSelectionOpenBuilder, CommandSelectionOpenBuilder> configuration)
    {
        var selectionBuilder = new CommandSelectionOpenBuilder();
        _selection = Option<CommandSelection>.Some(configuration(selectionBuilder).Build());

        return this;
    }

}