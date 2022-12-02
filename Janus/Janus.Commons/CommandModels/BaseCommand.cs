using Janus.Commons.SchemaModels;

namespace Janus.Commons.CommandModels;

/// <summary>
/// Base command class
/// </summary>
public abstract class BaseCommand
{
    protected readonly TableauId _onTableauId;
    protected readonly string _name;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="onTableauId">The command's starting tableau</param>
    /// <param name="name">Identifier for this command</param>
    protected BaseCommand(TableauId onTableauId, string? name = null)
    {
        if (onTableauId is null)
        {
            throw new ArgumentException($"'{nameof(onTableauId)}' cannot be null or empty.", nameof(onTableauId));
        }
        _name = name ?? Guid.NewGuid().ToString();
        _onTableauId = onTableauId;
    }

    /// <summary>
    /// The command's starting tableau
    /// </summary>
    public TableauId OnTableauId => _onTableauId;

    /// <summary>
    /// Identifier for this command
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// Checks if the command can be run on the given data source
    /// </summary>
    /// <param name="dataSource"></param>
    /// <returns></returns>
    public abstract Result IsValidForDataSource(DataSource dataSource);
}