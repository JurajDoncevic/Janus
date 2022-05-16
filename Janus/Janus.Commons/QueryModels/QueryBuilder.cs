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
            throw new TableauDoesNotExistException(tableauId, dataSource.Name);

        return new QueryBuilder(tableauId, dataSource);
    }

    public QueryBuilder WithSelection(Func<SelectionBuilder, SelectionBuilder> configuration)
    {
        var builder = new SelectionBuilder(_dataSource, _referencedTableaus);
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

    public QueryBuilder WithJoining(Func<JoiningBuilder, JoiningBuilder> configuration)
    {
        var builder = new JoiningBuilder(_dataSource);
        builder = configuration(builder);
        _joining = Option<Joining>.Some(builder.Build());
        return this;
    }

}

public class SelectionBuilder
{
    private string _expression;
    private readonly DataSource _dataSource;
    private readonly HashSet<string> _referencedTableaus;

    internal SelectionBuilder(DataSource dataSource,  HashSet<string> referencedTableaus)
    {
        _expression = "";
        _dataSource = dataSource;
        _referencedTableaus = referencedTableaus;
    }

    public SelectionBuilder WithExpression(string expression)
    {
        // TODO: do checks
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
            throw new AttributeDoesNotExistException(attributeId, _dataSource.Name);
        if(!_dataSource.Schemas
                       .SelectMany(schema => schema.Tableaus)
                       .Where(tableau => _referencedTableaus.Contains(tableau.Id))
                       .Any(tableau => tableau.Attributes.Any(attribute => attribute.Id.Equals(attributeId))))
            throw new AttributeNotInReferencedTableausException(attributeId);
        if (_projectionAttributes.Contains(attributeId))
            throw new DuplicateAttributeAssignedToProjectionException(attributeId);

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
    private readonly DataSource _dataSource;
    private Joining _joining;
    // TODO: check references, check circular joins
    internal JoiningBuilder(DataSource dataSource!!)
    {
        _dataSource = dataSource;
        _joining = new Joining();
    }

    public JoiningBuilder AddJoin(Join join)
    {
        if (!_dataSource.ContainsAttribute(join.ForeignKeyAttributeId))
            throw new AttributeDoesNotExistException(join.ForeignKeyAttributeId, _dataSource.Name);
        if(!_dataSource.ContainsAttribute(join.PrimaryKeyAttributeId))
            throw new AttributeDoesNotExistException(join.PrimaryKeyAttributeId, _dataSource.Name);
        if(join.ForeignKeyTableauId == join.PrimaryKeyTableauId)
            throw new SelfJoinNotSupportedException(join.PrimaryKeyTableauId);
        if(DoesCreateCycle(_joining, join))
            throw new CyclicJoinNotSupportedException(join.ForeignKeyTableauId, join.ForeignKeyAttributeId, join.PrimaryKeyTableauId, join.PrimaryKeyAttributeId);
        if (_joining.Joins.Contains(join))
            throw new DuplicateJoinNotSupportedException(join.ForeignKeyTableauId, join.ForeignKeyAttributeId, join.PrimaryKeyTableauId, join.PrimaryKeyAttributeId);
        _joining.AddJoin(join);

        return this;
    }

    private bool DoesCreateCycle(Joining joining, Join join)
    {
        return false;
    }

    public Joining Build()
    {
        return _joining;
    }

}


