using Janus.Commons.SchemaModels;

namespace Janus.Commons.CommandModels;

/// <summary>
/// Builder class to internally construct a delete command without validation on a data source
/// </summary>
public sealed class DeleteCommandOpenBuilder
{
    private readonly TableauId _onTableauId;
    private Option<CommandSelection> _selection;
    private string _commandName;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="onTableauId">Command's starting tableau</param>
    private DeleteCommandOpenBuilder(TableauId onTableauId)
    {
        _onTableauId = onTableauId;
        _selection = Option<CommandSelection>.None;
        _commandName = Guid.NewGuid().ToString(); // pre-empt null
    }

    /// <summary>
    /// Builds the specified delete command
    /// </summary>
    /// <returns></returns>
    public DeleteCommand Build()
        => new DeleteCommand(_onTableauId, _selection, _commandName);

    /// <summary>
    /// Initializes the delete command open builder
    /// </summary>
    /// <param name="onTableauId">Command's starting tableau</param>
    /// <returns></returns>
    public static DeleteCommandOpenBuilder InitOpenDelete(TableauId onTableauId)
    {
        return new DeleteCommandOpenBuilder(onTableauId);
    }

    /// <summary>
    /// Initializes the delete command open builder
    /// </summary>
    /// <param name="onTableauId">Command's starting tableau</param>
    /// <returns></returns>
    public static DeleteCommandOpenBuilder InitOpenDelete(string onTableauId)
        => InitOpenDelete(TableauId.From(onTableauId));

    /// <summary>
    /// Sets the delete command's selection clause
    /// </summary>
    /// <param name="configuration">Selection configuration</param>
    /// <returns></returns>
    public DeleteCommandOpenBuilder WithSelection(Func<CommandSelectionOpenBuilder, CommandSelectionOpenBuilder> configuration)
    {
        var selectionBuilder = new CommandSelectionOpenBuilder();
        _selection = Option<CommandSelection>.Some(configuration(selectionBuilder).Build());

        return this;
    }

    /// <summary>
    /// Sets the delete command name
    /// </summary>
    /// <param name="name">Command name</param>
    /// <returns></returns>
    public DeleteCommandOpenBuilder WithName(string name)
    {
        _commandName = name ?? _commandName;
        return this;
    }
}
