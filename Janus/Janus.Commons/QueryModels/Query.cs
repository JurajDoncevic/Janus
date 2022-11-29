using Janus.Commons.SchemaModels;

namespace Janus.Commons.QueryModels;

/// <summary>
/// Describes a query
/// </summary>
public sealed class Query
{
    private readonly string _name;
    private Option<Projection> _projection;
    private Option<Selection> _selection;
    private Option<Joining> _joining;
    private TableauId _onTableauId;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="onTableuId">Initial query tableau</param>
    /// <param name="projection">Projection clause</param>
    /// <param name="selection">Selection clause</param>
    /// <param name="joining">Joining clause</param>
    internal Query(TableauId onTableuId, Option<Projection> projection, Option<Selection> selection, Option<Joining> joining, string? name = null)
    {
        _onTableauId = onTableuId;
        _projection = projection;
        _selection = selection;
        _joining = joining;
        _name = string.IsNullOrEmpty(name) ? Guid.NewGuid().ToString() : name;
    }

    /// <summary>
    /// Optional projection clause
    /// </summary>
    public Option<Projection> Projection => _projection;

    /// <summary>
    /// Optional selection clause
    /// </summary>
    public Option<Selection> Selection => _selection;

    /// <summary>
    /// Optional joining clause
    /// </summary>
    public Option<Joining> Joining => _joining;

    /// <summary>
    /// Tableau on which the query is initialized
    /// </summary>
    public TableauId OnTableauId { get => _onTableauId; set => _onTableauId = value; }

    /// <summary>
    /// Identifier for this query and its possible results
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// Set of referenced tableaus in this query
    /// </summary>
    public HashSet<TableauId> ReferencedTableauIds =>
        _joining.Match(
            joining => joining.Joins
                              .Map(j => new List<TableauId> { j.ForeignKeyTableauId, j.PrimaryKeyTableauId })
                              .SelectMany(tblIds => tblIds),
            () => Enumerable.Empty<TableauId>()
            ).Fold(
                Enumerable.Empty<TableauId>().Append(OnTableauId),
                (tableauId, referencedTableauIds) => referencedTableauIds.Append(tableauId)
            ).ToHashSet();


    public override bool Equals(object? obj)
    {
        return obj is Query query &&
               _name.Equals(query.Name) &&
               EqualityComparer<Option<Projection>>.Default.Equals(_projection, query._projection) &&
               EqualityComparer<Option<Selection>>.Default.Equals(_selection, query._selection) &&
               EqualityComparer<Option<Joining>>.Default.Equals(_joining, query._joining) &&
               _onTableauId.Equals(query._onTableauId);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_projection, _selection, _joining, _onTableauId, _name);
    }


    /// <summary>
    /// Checks validity of query over a data source
    /// </summary>
    /// <param name="dataSource">Data source over which the query should be run</param>
    /// <returns>Validation result</returns>
    public Result IsValidForDataSource(DataSource dataSource)
     => Results.AsResult(() =>
     {
         if (dataSource is null)
         {
             throw new ArgumentNullException(nameof(dataSource));
         }

         QueryModelBuilder.InitQueryOnDataSource(_onTableauId, dataSource)
             .WithJoining(configuration =>
                 _joining.Match(
                     joining => joining.Joins.Fold(configuration, (join, conf) => conf.AddJoin(join.ForeignKeyAttributeId, join.PrimaryKeyAttributeId)),
                     () => configuration
                     ))
             .WithProjection(configuration =>
                 _projection.Match(
                     projection => projection.IncludedAttributeIds.Fold(configuration, (attrId, conf) => conf.AddAttribute(attrId)),
                     () => configuration
                     ))
             .WithSelection(configuration =>
                 _selection.Match(
                     selection => configuration.WithExpression(selection.Expression),
                     () => configuration
                     ))
             .Build();

         return true;
     });



}


