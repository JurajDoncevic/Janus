using Janus.Commons.QueryModels.Exceptions;
using Janus.Commons.SchemaModels;

namespace Janus.Commons.QueryModels;

public class QueryBuilder
{
    private Option<Projection> _projection;
    private Option<Selection> _selection;
    private Option<Joining> _joining;
    private readonly string _queryOnTableauId;
    private readonly DataSource _dataSource;
    private HashSet<string> _referencedTableaus;

    private QueryBuilder(string queryOnTableauId!!, DataSource dataSource!!)
    {
        _projection = Option<Projection>.None;
        _selection = Option<Selection>.None;
        _joining = Option<Joining>.None;
        _queryOnTableauId = queryOnTableauId;
        _dataSource = dataSource;
        _referencedTableaus = new HashSet<string>();
        _referencedTableaus.Add(queryOnTableauId);
    }

    public static QueryBuilder InitQueryOnTableau(string tableauId, DataSource dataSource)
    {
        if(!dataSource.ContainsTableau(tableauId))
            throw new ReferencedTableauDoesNotExistException(tableauId, dataSource.Name);

        return new QueryBuilder(tableauId, dataSource);
    }

    public QueryBuilder WithSelection(Func<SelectionBuilder, SelectionBuilder> configuration)
    {
        var builder = new SelectionBuilder(_dataSource);
        builder = configuration(builder);
        _selection = Option<Selection>.Some(builder.Build());
        return this;
    }

    public QueryBuilder WithProjection(Func<ProjectionBuilder, ProjectionBuilder> configuration)
    {
        var builder = new ProjectionBuilder(_dataSource, _referencedTableaus);
        builder = configuration(builder);
        _projection = Option<Projection>.Some(builder.Build());
        return this;
    }

}

public class SelectionBuilder
{
    private string _expression;
    private readonly DataSource _dataSource;

    internal SelectionBuilder(DataSource dataSource)
    {
        _expression = "";
        _dataSource = dataSource;
    }

    public SelectionBuilder WithExpression(string expression)
    {
        _expression = expression;
        return this;
    }

    internal Selection Build()
    {
        return new Selection(_expression);
    }
}

public class ProjectionBuilder
{
    private readonly DataSource _dataSource;
    private HashSet<string> _projectionAttributes;
    private readonly HashSet<string> _referencedTableaus;

    internal ProjectionBuilder(DataSource dataSource!!, HashSet<string> referencedTableaus!!)
    {
        _dataSource = dataSource;
        _projectionAttributes = new HashSet<string>();
        _referencedTableaus = referencedTableaus;
    }

    public ProjectionBuilder AddAttribute(string attributeId)
    {
        if (!_dataSource.ContainsAttribute(attributeId))
            throw new ReferencedAttributeDoesNotExistException(attributeId, _dataSource.Name);
        if(!_dataSource.Schemas
                       .SelectMany(schema => schema.Tableaus)
                       .Where(tableau => _referencedTableaus.Contains(tableau.Id))
                       .Any(tableau => tableau.Attributes.Any(attribute => attribute.Id.Equals(attributeId))))
            throw new ReferencedAttributeNotInReferencedTableausException(attributeId);
        if (_projectionAttributes.Contains(attributeId))
            throw new AttributeAlreadyAssignedToProjectionException(attributeId);

        _projectionAttributes.Add(attributeId);

        return this;
    }

    internal Projection Build()
    {
        return new Projection(_projectionAttributes);
    }
}

public class JoiningBuilder
{
    // TODO: check references, check circular joins
}


