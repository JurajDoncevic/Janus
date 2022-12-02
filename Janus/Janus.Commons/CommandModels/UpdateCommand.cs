using Janus.Commons.SchemaModels;

namespace Janus.Commons.CommandModels;

/// <summary>
/// Describes an update command
/// </summary>
public sealed class UpdateCommand : BaseCommand
{
    private readonly Mutation _mutation;
    private readonly Option<CommandSelection> _selection;

    /// <summary>
    /// Mutation clause
    /// </summary>
    public Mutation Mutation => _mutation;

    /// <summary>
    /// Selection clause
    /// </summary>
    public Option<CommandSelection> Selection => _selection;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="onTableauId">Starting tableau</param>
    /// <param name="mutation">Mutation clause</param>
    /// <param name="selection">Selection clause</param>
    internal UpdateCommand(TableauId onTableauId, Mutation mutation, Option<CommandSelection> selection, string? name = null) : base(onTableauId, name)
    {
        if (onTableauId is null)
        {
            throw new ArgumentException($"'{nameof(onTableauId)}' cannot be null or empty.", nameof(onTableauId));
        }

        _mutation = mutation ?? throw new ArgumentNullException(nameof(mutation));
        _selection = selection;
    }

    public override Result IsValidForDataSource(DataSource dataSource)
        => Results.AsResult(() =>
        {
            UpdateCommandBuilder.InitOnDataSource(_onTableauId, dataSource)
                .WithMutation(configuration => configuration.WithValues(_mutation.ValueUpdates.ToDictionary(kv => kv.Key, kv => kv.Value)))
                .WithSelection(configuration =>
                    _selection.Match(
                        selection => configuration.WithExpression(selection.Expression),
                        () => configuration
                        ))
                .Build();

            return true;
        });

    public override bool Equals(object? obj)
    {
        return obj is UpdateCommand command &&
               _name.Equals(command.Name) &&
               _onTableauId.Equals(command._onTableauId) &&
               EqualityComparer<Mutation>.Default.Equals(_mutation, command._mutation) &&
               EqualityComparer<Option<CommandSelection>>.Default.Equals(_selection, command._selection);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_onTableauId, _mutation, _selection);
    }

}
