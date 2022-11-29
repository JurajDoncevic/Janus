using Janus.Commons.QueryModels.Validation;
using Janus.Commons.SchemaModels;
using Janus.Commons.SelectionExpressions;

namespace Janus.Commons.QueryModels.Building;

/// <summary>
/// Builder class for query selection
/// </summary>
public sealed class SelectionBuilder
{
    private SelectionExpression? _expression;
    private readonly DataSource _dataSource;
    private readonly HashSet<TableauId> _referencedTableauIds;
    internal bool IsConfigured => _expression != null;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dataSource">Data source on which the query will be executed</param>
    /// <param name="referencedTableauIds">Ids of tableaus referenced in the query</param>
    internal SelectionBuilder(DataSource dataSource, HashSet<TableauId> referencedTableauIds)
    {
        _expression = null;
        _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
        _referencedTableauIds = referencedTableauIds ?? throw new ArgumentNullException(nameof(referencedTableauIds));
    }

    /// <summary>
    /// Creates the selection expression
    /// </summary>
    /// <param name="expression"></param>
    /// <returns>SelectionBuilder</returns>
    public SelectionBuilder WithExpression(SelectionExpression expression)
    {
        var referencableAttrs = _referencedTableauIds.Select(tId => tId.NameTuple)
                                                     .SelectMany(names => _dataSource[names.schemaName][names.tableauName].Attributes.Map(a => (a.Id, a.DataType)))
                                                     .ToDictionary(x => x.Id, x => x.DataType);
        SelectionValidationMethods.CheckSelectionAttributeReferences(expression, referencableAttrs.Keys.ToHashSet());
        SelectionValidationMethods.CheckSelectionAttributeTypesOnComparison(expression, referencableAttrs);


        _expression = expression;
        return this;
    }

    /// <summary>
    /// Builds the specified selection
    /// </summary>
    /// <returns></returns>
    internal Selection Build()
    {
        return new Selection(_expression ?? new TrueLiteral());
    }
}





