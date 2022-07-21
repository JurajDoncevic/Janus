using Janus.Commons.CommandModels.Exceptions;
using Janus.Commons.SelectionExpressions;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.Commons.CommandModels;

/// <summary>
/// Builder class to internally construct an update command without validation on a data source
/// </summary>
public class UpdateCommandOpenBuilder
{
    private readonly string _onTableauId;
    private Option<Mutation> _mutation;
    private Option<CommandSelection> _selection;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="onTableauId"></param>
    private UpdateCommandOpenBuilder(string onTableauId)
    {
        _onTableauId = onTableauId;
    }

    /// <summary>
    /// Initializes an update command open builder
    /// </summary>
    /// <param name="onTableauId">Starting tableau</param>
    /// <returns></returns>
    public static UpdateCommandOpenBuilder InitOpenUpdate(string onTableauId)
    {
        return new UpdateCommandOpenBuilder(onTableauId);
    }

    /// <summary>
    /// Sets the mutation clause. <b>Mutation attributes are set by their name.</b> Don't use attribute ids.
    /// </summary>
    /// <param name="configuration">Mutation configuration</param>
    /// <returns></returns>
    public UpdateCommandOpenBuilder WithMutation(Func<MutationOpenBuilder, MutationOpenBuilder> configuration)
    {
        var mutationBuilder = new MutationOpenBuilder();

        _mutation = Option<Mutation>.Some(configuration(mutationBuilder).Build());

        return this;
    }

    /// <summary>
    /// Sets the selection clause
    /// </summary>
    /// <param name="configuration">Selection configuration</param>
    /// <returns></returns>
    public UpdateCommandOpenBuilder WithSelection(Func<CommandSelectionOpenBuilder, CommandSelectionOpenBuilder> configuration)
    {
        var selectionBuilder = new CommandSelectionOpenBuilder();
        _selection = Option<CommandSelection>.Some(configuration(selectionBuilder).Build());

        return this;
    }

    /// <summary>
    /// Builds the update command
    /// </summary>
    /// <returns></returns>
    /// <exception cref="MutationNotSetException"></exception>
    public UpdateCommand Build()
    {
        if (!_mutation)
            throw new MutationNotSetException();
        return new UpdateCommand(_onTableauId,
                                 _mutation.Value,
                                 _selection);

    }
}

/// <summary>
/// Mutation clause open builder
/// </summary>
public class MutationOpenBuilder
{
    private Option<Dictionary<string, object?>> _valueUpdates;

    /// <summary>
    /// Constructor
    /// </summary>
    internal MutationOpenBuilder()
    {
    }

    /// <summary>
    /// Sets the values for updating. <b>Attributes are referenced by names.</b> Don't use ids.
    /// </summary>
    /// <param name="valueUpdates">Attributes to update to values</param>
    /// <returns></returns>
    public MutationOpenBuilder WithValues(Dictionary<string, object?> valueUpdates!!)
    {
        _valueUpdates = Option<Dictionary<string, object?>>.Some(valueUpdates);
        return this;
    }

    /// <summary>
    /// Builds the mutation clause
    /// </summary>
    /// <returns></returns>
    internal Mutation Build()
        => _valueUpdates
           ? new Mutation(_valueUpdates.Value)
           : new Mutation(new());
}

/// <summary>
/// Selection clause open builder
/// </summary>
public class CommandSelectionOpenBuilder
{
    private Option<SelectionExpression> _expression;

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
    public CommandSelectionOpenBuilder WithExpression(SelectionExpression expression!!)
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