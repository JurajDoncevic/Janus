using Janus.Commons.SchemaModels;

namespace Janus.Commons.CommandModels;

/// <summary>
/// Base command class
/// </summary>
public abstract class BaseCommand
{
    protected readonly string _onTableauId;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="onTableauId">The command's starting tableau</param>
    protected BaseCommand(string onTableauId)
    {
        if (string.IsNullOrEmpty(onTableauId))
        {
            throw new ArgumentException($"'{nameof(onTableauId)}' cannot be null or empty.", nameof(onTableauId));
        }

        _onTableauId = onTableauId;
    }

    /// <summary>
    /// The command's starting tableau
    /// </summary>
    public string OnTableauId => _onTableauId;

    /// <summary>
    /// Checks if the command can be run on the given data source
    /// </summary>
    /// <param name="dataSource"></param>
    /// <returns></returns>
    public abstract Result IsValidForDataSource(DataSource dataSource);
}