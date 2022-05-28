using Janus.Commons.CommandModels.Exceptions;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels.Exceptions;
using Janus.Commons.SchemaModels;
using Janus.Commons.SelectionExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.CommandModels;


public class UpdateCommandOpenBuilder 
{
    private readonly string _onTableauId;
    private Mutation? _mutation;
    private CommandSelection? _selection;

    internal UpdateCommandOpenBuilder(string onTableauId)
    {
        _onTableauId = onTableauId;
    }

    public static UpdateCommandOpenBuilder InitOpenUpdate(string onTableauId)
    {
        return new UpdateCommandOpenBuilder(onTableauId);
    }

    public UpdateCommandOpenBuilder WithMutation(Func<MutationOpenBuilder, MutationOpenBuilder> configuration)
    {
        var mutationBuilder = new MutationOpenBuilder();

        _mutation = configuration(mutationBuilder).Build();

        return this;
    }

    public UpdateCommandOpenBuilder WithSelection(Func<CommandSelectionOpenBuilder, CommandSelectionOpenBuilder> configuration)
    {
        var selectionBuilder = new CommandSelectionOpenBuilder();
        _selection = configuration(selectionBuilder).Build();

        return this;
    }

    public UpdateCommand Build()
    {
        if (_mutation == null)
            throw new MutationNotSetException();
        return new UpdateCommand(_onTableauId,
                                 _mutation,
                                 _selection == null ? Option<CommandSelection>.None : Option<CommandSelection>.Some(_selection));

    }
}

public class MutationOpenBuilder
{
    private Dictionary<string, object>? _valueUpdates;

    internal MutationOpenBuilder()
    {
    }

    public MutationOpenBuilder WithValues(Dictionary<string, object> valueUpdates)
    {
        _valueUpdates = valueUpdates;
        return this;
    }

    internal Mutation Build()
        => _valueUpdates != null
           ? new Mutation(_valueUpdates)
           : new Mutation(new());
}


public class CommandSelectionOpenBuilder
{
    private SelectionExpression _expression;
    internal bool IsConfigured => _expression != null;

    /// <summary>
    /// Constructor
    /// </summary>
    internal CommandSelectionOpenBuilder()
    {
        _expression = null;
    }

    /// <summary>
    /// Creates the selection expression
    /// </summary>
    /// <param name="expression"></param>
    /// <returns>SelectionBuilder</returns>
    public CommandSelectionOpenBuilder WithExpression(SelectionExpression expression!!)
    {
        _expression = expression;
        return this;
    }

    /// <summary>
    /// Builds the specified selection
    /// </summary>
    /// <returns></returns>
    internal CommandSelection Build()
    {
        return new CommandSelection(_expression ?? new TrueLiteral());
    }
}