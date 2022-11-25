

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
    private readonly string _onTableauId;
    private readonly DataSource _dataSource;
    private Option<CommandSelection> _selection;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="onTableauId">Command's starting tableau</param>
    /// <param name="dataSource">Target data source for the command</param>
    private DeleteCommandBuilder(string onTableauId, DataSource dataSource)
    {
        if (string.IsNullOrEmpty(onTableauId))
        {
            throw new ArgumentException($"'{nameof(onTableauId)}' cannot be null or empty.", nameof(onTableauId));
        }

        _onTableauId = onTableauId;
        _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
        _selection = Option<CommandSelection>.None;
    }

    /// <summary>
    /// Initializes a delete command builder on a tableau from a data source
    /// </summary>
    /// <param name="onTableauId"></param>
    /// <param name="dataSource"></param>
    /// <returns></returns>
    /// <exception cref="TableauDoesNotExistException"></exception>
    public static IPostInitDeleteCommandBuilder InitOnDataSource(string onTableauId, DataSource dataSource)
    {
        if (string.IsNullOrEmpty(onTableauId))
        {
            throw new ArgumentException($"'{nameof(onTableauId)}' cannot be null or empty.", nameof(onTableauId));
        }

        if (dataSource is null)
        {
            throw new ArgumentNullException(nameof(dataSource));
        }

        if (!dataSource.ContainsTableau(onTableauId))
            throw new TableauDoesNotExistException(onTableauId, dataSource.Name);

        (string _, string schemaName, string tableauName) = Utils.GetNamesFromTableauId(onTableauId);

        if (!dataSource[schemaName][tableauName].UpdateSets.Any(us => us.AttributeNames.SequenceEqual(dataSource[schemaName][tableauName].AttributeNames)))
            throw new CommandAllowedOnTableauWideUpdateSetException();

        return new DeleteCommandBuilder(onTableauId, dataSource);
    }


    public IPostInitDeleteCommandBuilder WithSelection(Func<CommandSelectionBuilder, CommandSelectionBuilder> configuration)
    {
        var selectionBuilder = new CommandSelectionBuilder(_dataSource, _onTableauId);
        _selection = Option<CommandSelection>.Some(configuration(selectionBuilder).Build());

        return this;
    }

    public DeleteCommand Build()
    {
        return new DeleteCommand(_onTableauId, _selection);
    }
}
