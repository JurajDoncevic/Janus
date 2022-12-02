

using Janus.Commons.CommandModels.Exceptions;
using Janus.Commons.QueryModels.Exceptions;
using Janus.Commons.SchemaModels;

namespace Janus.Commons.CommandModels;

#region BUILDER SEQUENCE INTERFACES
public interface IPostInitDeleteCommandBuilder
{
    /// <summary>
    /// Sets the selection clause of the delete command
    /// </summary>
    /// <param name="configuration">Selection configuration</param>
    /// <returns></returns>
    IPostInitDeleteCommandBuilder WithSelection(Func<CommandSelectionBuilder, CommandSelectionBuilder> configuration);
    /// <summary>
    /// Sets the delete command name
    /// </summary>
    /// <param name="name">Command name</param>
    /// <returns></returns>
    IPostInitDeleteCommandBuilder WithName(string name);
    /// <summary>
    /// Builds the specified delete command
    /// </summary>
    /// <returns>Delete command</returns>
    DeleteCommand Build();
}
#endregion

/// <summary>
/// Builder for the delete command
/// </summary>
public sealed class DeleteCommandBuilder : IPostInitDeleteCommandBuilder
{
    private readonly TableauId _onTableauId;
    private readonly DataSource _dataSource;
    private Option<CommandSelection> _selection;
    private string _commandName;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="onTableauId">Command's starting tableau</param>
    /// <param name="dataSource">Target data source for the command</param>
    private DeleteCommandBuilder(TableauId onTableauId, DataSource dataSource)
    {
        if (onTableauId is null)
        {
            throw new ArgumentException($"'{nameof(onTableauId)}' cannot be null or empty.", nameof(onTableauId));
        }

        _onTableauId = onTableauId;
        _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
        _selection = Option<CommandSelection>.None;
        _commandName = Guid.NewGuid().ToString(); // pre-empt null
    }

    /// <summary>
    /// Initializes a delete command builder on a tableau from a data source
    /// </summary>
    /// <param name="onTableauId"></param>
    /// <param name="dataSource"></param>
    /// <returns></returns>
    /// <exception cref="TableauDoesNotExistException"></exception>
    public static IPostInitDeleteCommandBuilder InitOnDataSource(TableauId onTableauId, DataSource dataSource)
    {
        if (onTableauId is null)
        {
            throw new ArgumentException($"'{nameof(onTableauId)}' cannot be null or empty.", nameof(onTableauId));
        }

        if (dataSource is null)
        {
            throw new ArgumentNullException(nameof(dataSource));
        }

        if (!dataSource.ContainsTableau(onTableauId))
            throw new TableauDoesNotExistException(onTableauId, dataSource.Name);

        (string _, string schemaName, string tableauName) = onTableauId.NameTuple;

        if (!dataSource[schemaName][tableauName].UpdateSets.Any(us => us.AttributeNames.SequenceEqual(dataSource[schemaName][tableauName].AttributeNames)))
            throw new CommandAllowedOnTableauWideUpdateSetException();

        return new DeleteCommandBuilder(onTableauId, dataSource);
    }

    /// <summary>
    /// Initializes a delete command builder on a tableau from a data source
    /// </summary>
    /// <param name="onTableauId"></param>
    /// <param name="dataSource"></param>
    /// <returns></returns>
    /// <exception cref="TableauDoesNotExistException"></exception>
    public static IPostInitDeleteCommandBuilder InitOnDataSource(string onTableauId, DataSource dataSource)
        => InitOnDataSource(TableauId.From(onTableauId), dataSource);

    public IPostInitDeleteCommandBuilder WithName(string name)
    {
        _commandName = name ?? _commandName;
        return this;
    }

    public IPostInitDeleteCommandBuilder WithSelection(Func<CommandSelectionBuilder, CommandSelectionBuilder> configuration)
    {
        var selectionBuilder = new CommandSelectionBuilder(_dataSource, _onTableauId);
        _selection = Option<CommandSelection>.Some(configuration(selectionBuilder).Build());

        return this;
    }

    public DeleteCommand Build()
    {
        return new DeleteCommand(_onTableauId, _selection, _commandName);
    }
}
