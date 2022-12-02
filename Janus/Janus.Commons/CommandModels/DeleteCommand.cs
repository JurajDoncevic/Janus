using Janus.Commons.SchemaModels;

namespace Janus.Commons.CommandModels;

/// <summary>
/// Describes a delete command
/// </summary>
public sealed class DeleteCommand : BaseCommand
{
    private readonly Option<CommandSelection> _selection;

    /// <summary>
    /// Selection clause
    /// </summary>
    public Option<CommandSelection> Selection => _selection;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="onTableauId">Starting tableau</param>
    /// <param name="selection">Selection clause</param>
    internal DeleteCommand(TableauId onTableauId, Option<CommandSelection> selection, string? name = null) : base(onTableauId, name)
    {
        _selection = selection;
    }

    public override bool Equals(object? obj)
    {
        return obj is DeleteCommand command &&
               _name.Equals(command.Name) &&
               _onTableauId.Equals(command._onTableauId) &&
               _selection.Equals(command._selection);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_onTableauId, _selection);
    }

    public override Result IsValidForDataSource(DataSource dataSource)
        => Results.AsResult(() =>
        {
            DeleteCommandBuilder.InitOnDataSource(_onTableauId, dataSource)
                .WithSelection(conf =>
                    _selection.Match(
                        selection => conf.WithExpression(selection.Expression),
                        () => conf
                    ))
                .Build();
            return true;
        });
}
