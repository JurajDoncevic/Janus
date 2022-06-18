using Janus.Commons.CommandModels.Exceptions;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels.Exceptions;
using Janus.Commons.SchemaModels;
using Janus.Commons.SelectionExpressions;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.Commons.CommandModels;

#region BUILDER SEQUENCE INTERFACES
public interface IPostInitUpdateCommandBuilder
{
    /// <summary>
    /// Sets the mutation clause
    /// </summary>
    /// <param name="configuration">Mutation configuration</param>
    /// <returns></returns>
    IPostMutationUpdateCommandBuilder WithMutation(Func<MutationBuilder, MutationBuilder> configuration);
}

public interface IPostMutationUpdateCommandBuilder
{
    /// <summary>
    /// Sets the selection clause
    /// </summary>
    /// <param name="configuration">Selection configuration</param>
    /// <returns></returns>
    IPostSelectionUpdateCommandBuilder WithSelection(Func<CommandSelectionBuilder, CommandSelectionBuilder> configuration);

    /// <summary>
    /// Builds the specified update command
    /// </summary>
    /// <returns></returns>
    UpdateCommand Build();
}

public interface IPostSelectionUpdateCommandBuilder
{
    /// <summary>
    /// Builds the specified update command
    /// </summary>
    /// <returns></returns>
    UpdateCommand Build();
}
#endregion

public class UpdateCommandBuilder : IPostInitUpdateCommandBuilder, IPostMutationUpdateCommandBuilder, IPostSelectionUpdateCommandBuilder
{
    private readonly string _onTableauId;
    private readonly DataSource _dataSource;
    private Option<Mutation> _mutation;
    private Option<CommandSelection> _selection;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="onTableauId">Starting tableau</param>
    /// <param name="dataSource">Target data source</param>
    internal UpdateCommandBuilder(string onTableauId, DataSource dataSource)
    {
        _onTableauId = onTableauId;
        _dataSource = dataSource;
    }

    /// <summary>
    /// Initializes the update command builder with starting tableau from the given data source
    /// </summary>
    /// <param name="onTableauId">Starting tableau</param>
    /// <param name="dataSource">Target data source</param>
    /// <returns></returns>
    /// <exception cref="TableauDoesNotExistException"></exception>
    public static IPostInitUpdateCommandBuilder InitOnDataSource(string onTableauId, DataSource dataSource)
    {
        if (!dataSource.ContainsTableau(onTableauId))
            throw new TableauDoesNotExistException(onTableauId, dataSource.Name);

        return new UpdateCommandBuilder(onTableauId, dataSource);
    }

    public IPostMutationUpdateCommandBuilder WithMutation(Func<MutationBuilder, MutationBuilder> configuration)
    {
        var mutationBuilder = new MutationBuilder(_onTableauId, _dataSource);

        _mutation = Option<Mutation>.Some(configuration(mutationBuilder).Build());

        return this;
    }

    public IPostSelectionUpdateCommandBuilder WithSelection(Func<CommandSelectionBuilder, CommandSelectionBuilder> configuration)
    {
        var selectionBuilder = new CommandSelectionBuilder(_dataSource, new() { _onTableauId });
        _selection = Option<CommandSelection>.Some(configuration(selectionBuilder).Build());

        return this;
    }

    public UpdateCommand Build()
    {
        if (!_mutation)
            throw new MutationNotSetException();
        return new UpdateCommand(_onTableauId,
                                 _mutation.Value,
                                 _selection);

    }
}

/// <summary>
/// Builder for the mutation clause
/// </summary>
public class MutationBuilder
{
    private Dictionary<string, object?>? _valueUpdates;
    private readonly string _onTableauId;
    private readonly DataSource _dataSource;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="onTableauId">Starting tableau</param>
    /// <param name="dataSource">Target data source</param>
    internal MutationBuilder(string onTableauId, DataSource dataSource)
    {
        _onTableauId = onTableauId;
        _dataSource = dataSource;
    }

    /// <summary>
    /// Sets the value updates in the mutation clause
    /// </summary>
    /// <param name="valueUpdates">Value updates. <b>Attributes are referenced as names, not ids</b></param>
    /// <returns></returns>
    /// <exception cref="AttributeNotInTargetTableauException"></exception>
    /// <exception cref="IncompatibleMutationDataTypesException"></exception>
    /// <exception cref="NullGivenForNonNullableAttributeException"></exception>
    public MutationBuilder WithValues(Dictionary<string, object?> valueUpdates!!)
    {
        (_, string schemaName, string tableauName) = Utils.GetNamesFromTableauId(_onTableauId);

        var referencedAttrNames = valueUpdates.Keys.ToHashSet();
        var referencableAttrNames = _dataSource[schemaName][tableauName].AttributeNames.ToHashSet();

        if (!referencedAttrNames.All(referencableAttrNames.Contains))
        {
            var invalidAttrRef = referencedAttrNames.Where(referencedAttr => !referencableAttrNames.Contains(referencedAttr)).First();
            throw new AttributeNotInTargetTableauException(invalidAttrRef, _onTableauId);
        }

        foreach (var referencedAttr in referencedAttrNames)
        {
            if (valueUpdates[referencedAttr] != null) // can't get null type
            {
                var referencedDataType = _dataSource[schemaName][tableauName][referencedAttr].DataType;
                var referencingDataType = valueUpdates[referencedAttr] != null
                                          ? TypeMappings.MapToDataType(valueUpdates[referencedAttr].GetType())
                                          : _dataSource[schemaName][tableauName][referencedAttr].DataType;
                if (referencedDataType != referencingDataType)
                {
                    throw new IncompatibleMutationDataTypesException(_onTableauId, referencedAttr, referencedDataType, referencingDataType);
                }
            }
        }

        foreach (var referencedAttr in referencableAttrNames.Intersect(valueUpdates.Keys))
        {
            if (!_dataSource[schemaName][tableauName][referencedAttr].IsNullable && valueUpdates[referencedAttr] == null)
                throw new NullGivenForNonNullableAttributeException(_onTableauId, referencedAttr);
        }

        foreach (var referencedAttrName in referencedAttrNames)
        {
            if (_dataSource[schemaName][tableauName][referencedAttrName].IsPrimaryKey)
            {
                throw new MutationOnPrimaryKeyNotAllowedException(referencedAttrName);
            }
        }

        _valueUpdates = valueUpdates;
        return this;
    }

    /// <summary>
    /// Builds the specified mutation clause
    /// </summary>
    /// <returns></returns>
    internal Mutation Build()
        => _valueUpdates != null
           ? new Mutation(_valueUpdates)
           : new Mutation(new());
}


public class CommandSelectionBuilder
{
    private Option<SelectionExpression> _expression;
    private readonly DataSource _dataSource;
    private readonly HashSet<string> _referencedTableauIds;
    internal bool IsConfigured => _expression.IsSome;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dataSource">Data source on which the query will be executed</param>
    /// <param name="referencedTableauIds">Ids of tableaus referenced in the query</param>
    internal CommandSelectionBuilder(DataSource dataSource!!, HashSet<string> referencedTableauIds!!)
    {
        _expression = Option<SelectionExpression>.None;
        _dataSource = dataSource;
        _referencedTableauIds = referencedTableauIds;
    }

    /// <summary>
    /// Creates the selection expression
    /// </summary>
    /// <param name="expression"></param>
    /// <returns>SelectionBuilder</returns>
    public CommandSelectionBuilder WithExpression(SelectionExpression expression!!)
    {
        var referencableAttrNames = _referencedTableauIds.Select(tId => Utils.GetNamesFromTableauId(tId))
                                                     .SelectMany(names => _dataSource[names.schemaName][names.tableauName].Attributes.Map(a => (a.Name, a.DataType)))
                                                     .ToDictionary(x => x.Name, x => x.DataType);

        CommandSelectionUtils.CheckAttributeReferences(expression, referencableAttrNames.Keys.ToHashSet());
        CommandSelectionUtils.CheckAttributeTypesOnComparison(expression, referencableAttrNames ?? new());


        _expression = Option<SelectionExpression>.Some(expression);
        return this;
    }

    /// <summary>
    /// Builds the specified selection
    /// </summary>
    /// <returns></returns>
    internal CommandSelection Build()
        => _expression
            ? new CommandSelection(_expression.Value)
            : new CommandSelection(FALSE());

}