using Janus.Commons.SchemaModels;

namespace Janus.Commons.CommandModels;
public class InsertCommand : BaseCommand
{
    private readonly Instantiation _instantiation;

    /// <summary>
    /// Instantiation clause
    /// </summary>
    public Instantiation Instantiation => _instantiation;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="onTableauId">Starting tableau</param>
    /// <param name="instantiation">Instantiation clause</param>
    internal InsertCommand(string onTableauId, Instantiation instantiation) : base(onTableauId)
    {
        if (string.IsNullOrEmpty(onTableauId))
        {
            throw new ArgumentException($"'{nameof(onTableauId)}' cannot be null or empty.", nameof(onTableauId));
        }

        _instantiation = instantiation ?? throw new ArgumentNullException(nameof(instantiation));
    }

    public override Result IsValidForDataSource(DataSource dataSource)
        => ResultExtensions.AsResult(() =>
        {
            InsertCommandBuilder.InitOnDataSource(_onTableauId, dataSource)
                .WithInstantiation(configuration => configuration.WithValues(_instantiation.TabularData))
                .Build();

            return true;
        });

    public override bool Equals(object? obj)
    {
        return obj is InsertCommand command &&
               _onTableauId.Equals(command._onTableauId) &&
               _instantiation.Equals(command._instantiation);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_onTableauId, _instantiation);
    }
}
