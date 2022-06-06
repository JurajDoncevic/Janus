using Janus.Commons.CommandModels.Exceptions;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels.Exceptions;
using Janus.Commons.SchemaModels;
using Janus.Commons.SelectionExpressions;
using static Janus.Commons.SelectionExpressions.SelectionExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.CommandModels;


internal class UpdateCommandOpenBuilder 
{
    private readonly string _onTableauId;
    private Option<Mutation> _mutation;
    private Option<CommandSelection> _selection;

    private UpdateCommandOpenBuilder(string onTableauId)
    {
        _onTableauId = onTableauId;
    }

    internal static UpdateCommandOpenBuilder InitOpenUpdate(string onTableauId)
    {
        return new UpdateCommandOpenBuilder(onTableauId);
    }

    internal UpdateCommandOpenBuilder WithMutation(Func<MutationOpenBuilder, MutationOpenBuilder> configuration)
    {
        var mutationBuilder = new MutationOpenBuilder();

        _mutation = Option<Mutation>.Some(configuration(mutationBuilder).Build());

        return this;
    }

    internal UpdateCommandOpenBuilder WithSelection(Func<CommandSelectionOpenBuilder, CommandSelectionOpenBuilder> configuration)
    {
        var selectionBuilder = new CommandSelectionOpenBuilder();
        _selection = Option<CommandSelection>.Some(configuration(selectionBuilder).Build());

        return this;
    }

    internal UpdateCommand Build()
    {
        if (!_mutation)
            throw new MutationNotSetException();
        return new UpdateCommand(_onTableauId,
                                 _mutation.Value,
                                 _selection);

    }
}

internal class MutationOpenBuilder
{
    private Option<Dictionary<string, object?>> _valueUpdates;

    internal MutationOpenBuilder()
    {
    }

    internal MutationOpenBuilder WithValues(Dictionary<string, object?> valueUpdates!!)
    {
        _valueUpdates = Option<Dictionary<string, object?>>.Some(valueUpdates);
        return this;
    }

    internal Mutation Build()
        => _valueUpdates
           ? new Mutation(_valueUpdates.Value)
           : new Mutation(new());
}


internal class CommandSelectionOpenBuilder
{
    private Option<SelectionExpression> _expression;
    internal bool IsConfigured => _expression.IsSome;

    /// <summary>
    /// Constructor
    /// </summary>
    internal CommandSelectionOpenBuilder()
    {
        _expression = Option<SelectionExpression>.Some(TRUE());
    }

    /// <summary>
    /// Creates the selection expression
    /// </summary>
    /// <param name="expression"></param>
    /// <returns>SelectionBuilder</returns>
    internal CommandSelectionOpenBuilder WithExpression(SelectionExpression expression!!)
    {
        _expression = Option<SelectionExpression>.Some(expression);
        return this;
    }

    /// <summary>
    /// Builds the specified selection
    /// </summary>
    /// <returns></returns>
    internal CommandSelection Build()
        => _expression
            ? new CommandSelection(_expression.Value)
            : new CommandSelection(TRUE());
}