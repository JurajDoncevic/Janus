using Janus.Commons.QueryModels.JsonConversion;
using Janus.Commons.SchemaModels;
using System.Text.Json.Serialization;

namespace Janus.Commons.QueryModels;

/// <summary>
/// Describes a query
/// </summary>
[JsonConverter(typeof(QueryJsonConverter))]
public class Query
{
    private Option<Projection> _projection;
    private Option<Selection> _selection;
    private Option<Joining> _joining;
    private string _onTableauId;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="onTableuId">Initial query tableau</param>
    /// <param name="projection">Projection clause</param>
    /// <param name="selection">Selection clause</param>
    /// <param name="joining">Joining clause</param>
    internal Query(string onTableuId, Option<Projection> projection, Option<Selection> selection, Option<Joining> joining)
    {
        _onTableauId = onTableuId;
        _projection = projection;
        _selection = selection;
        _joining = joining;
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
    public string OnTableauId { get => _onTableauId; set => _onTableauId = value; }

    public override bool Equals(object? obj)
    {
        return obj is Query query &&
               EqualityComparer<Option<Projection>>.Default.Equals(_projection, query._projection) &&
               EqualityComparer<Option<Selection>>.Default.Equals(_selection, query._selection) &&
               EqualityComparer<Option<Joining>>.Default.Equals(_joining, query._joining) &&
               _onTableauId == query._onTableauId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_projection, _selection, _joining, _onTableauId);
    }


    /// <summary>
    /// Checks validity of query over a data source
    /// </summary>
    /// <param name="dataSource">Data source over which the query should be run</param>
    /// <returns>Validation result</returns>
    public Result IsValidForDataSource(DataSource dataSource!!)
     => ResultExtensions.AsResult(() =>
     {
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


