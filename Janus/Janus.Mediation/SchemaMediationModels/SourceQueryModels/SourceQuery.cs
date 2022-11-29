using Janus.Commons.SchemaModels;

namespace Janus.Mediation.SchemaMediationModels.MediationQueryModels;
/// <summary>
/// Describes a tableau mediation source query
/// </summary>
public class SourceQuery
{
    private readonly TableauId _initialTableauId;
    private readonly Option<Joining> _joining;
    private readonly Projection _projection;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="initialTableauId">Initial query tableau id</param>
    /// <param name="joining">Optional query joining (joins)</param>
    /// <param name="projection">Query projection</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    internal SourceQuery(TableauId initialTableauId, Option<Joining> joining, Projection projection)
    {
        if (initialTableauId is null)
        {
            throw new ArgumentException($"'{nameof(initialTableauId)}' cannot be null or whitespace.", nameof(initialTableauId));
        }

        _initialTableauId = initialTableauId;
        _joining = joining;
        _projection = projection ?? throw new ArgumentNullException(nameof(projection));
    }

    /// <summary>
    /// Initial tableau id for the source query
    /// </summary>
    public TableauId InitialTableauId => _initialTableauId;
    /// <summary>
    /// Optional source query joining (joins)
    /// </summary>
    public Option<Joining> Joining => _joining;
    /// <summary>
    /// Source query projection
    /// </summary>
    public Projection Projection => _projection;
    /// <summary>
    /// All source tableau ids referenced in the source query
    /// </summary>
    public IReadOnlySet<TableauId> ReferencedTableauIds
        => Joining.Match(
            joining => joining.Joins.Fold(
                        new HashSet<TableauId>(new[] { _initialTableauId }),
                        (join, refdTableaus) => refdTableaus.Append(join.PrimaryKeyTableauId).Append(join.ForeignKeyTableauId).ToHashSet()
                        ),
            () => new HashSet<TableauId>(new[] { _initialTableauId })
            );

    public override bool Equals(object? obj)
    {
        return obj is SourceQuery query &&
               _initialTableauId == query._initialTableauId &&
               _joining.Equals(query._joining) &&
               _projection.Equals(query._projection);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_initialTableauId, _joining, _projection);
    }
}
