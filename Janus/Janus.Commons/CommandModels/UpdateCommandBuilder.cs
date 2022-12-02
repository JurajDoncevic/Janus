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

    /// <summary>
    /// Sets the update command name
    /// </summary>
    /// <param name="name">Command name</param>
    /// <returns></returns>
    IPostInitUpdateCommandBuilder WithName(string name);
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

public sealed class UpdateCommandBuilder : IPostInitUpdateCommandBuilder, IPostMutationUpdateCommandBuilder, IPostSelectionUpdateCommandBuilder
{
    private readonly TableauId _onTableauId;
    private readonly DataSource _dataSource;
    private Option<Mutation> _mutation;
    private Option<CommandSelection> _selection;
    private UpdateSet? _targetUpdateSet;
    private string _commandName;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="onTableauId">Starting tableau id</param>
    /// <param name="dataSource">Target data source</param>
    internal UpdateCommandBuilder(TableauId onTableauId, DataSource dataSource)
    {
        _onTableauId = onTableauId;
        _dataSource = dataSource;
        _commandName = Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Initializes the update command builder with starting tableau from the given data source
    /// </summary>
    /// <param name="onTableauId">Starting tableau</param>
    /// <param name="dataSource">Target data source</param>
    /// <returns></returns>
    /// <exception cref="TableauDoesNotExistException"></exception>
    public static IPostInitUpdateCommandBuilder InitOnDataSource(TableauId onTableauId, DataSource dataSource)
    {
        if (!dataSource.ContainsTableau(onTableauId))
            throw new TableauDoesNotExistException(onTableauId, dataSource.Name);

        return new UpdateCommandBuilder(onTableauId, dataSource);
    }

    /// <summary>
    /// Initializes the update command builder with starting tableau from the given data source
    /// </summary>
    /// <param name="onTableauId">Starting tableau</param>
    /// <param name="dataSource">Target data source</param>
    /// <returns></returns>
    /// <exception cref="TableauDoesNotExistException"></exception>
    public static IPostInitUpdateCommandBuilder InitOnDataSource(string onTableauId, DataSource dataSource)
        => InitOnDataSource(TableauId.From(onTableauId), dataSource);

    public IPostMutationUpdateCommandBuilder WithMutation(Func<MutationBuilder, MutationBuilder> configuration)
    {
        var mutationBuilder = new MutationBuilder(_onTableauId, _dataSource);

        _mutation = Option<Mutation>.Some(configuration(mutationBuilder).Build());
        _targetUpdateSet = mutationBuilder.ReferencedUpdateSet;
        return this;
    }

    public IPostSelectionUpdateCommandBuilder WithSelection(Func<CommandSelectionBuilder, CommandSelectionBuilder> configuration)
    {
        var selectionBuilder = new CommandSelectionBuilder(_dataSource, _onTableauId);
        _selection = Option<CommandSelection>.Some(configuration(selectionBuilder).Build());

        if (selectionBuilder.ReferencedUpdateSet is not null &&
           _targetUpdateSet is not null &&
           !selectionBuilder.ReferencedUpdateSet.IsEmpty() &&
           !_targetUpdateSet.Equals(selectionBuilder.ReferencedUpdateSet))
        {
            throw new UpdateSetMismatchBetweenClausesException(_targetUpdateSet, selectionBuilder.ReferencedUpdateSet, _onTableauId);
        }
        return this;
    }

    public IPostInitUpdateCommandBuilder WithName(string name)
    {
        _commandName = name ?? _commandName;
        return this;
    }

    public UpdateCommand Build()
    {
        if (!_mutation)
            throw new MutationNotSetException();
        return new UpdateCommand(_onTableauId,
                                 _mutation.Value,
                                 _selection,
                                 _commandName);

    }
}

/// <summary>
/// Builder for the mutation clause
/// </summary>
public sealed class MutationBuilder
{
    private Dictionary<string, object?>? _valueUpdates;
    private readonly TableauId _onTableauId;
    private readonly DataSource _dataSource;
    private UpdateSet? _referencedUpdateSet;

    internal UpdateSet? ReferencedUpdateSet => _referencedUpdateSet;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="onTableauId">Starting tableau</param>
    /// <param name="dataSource">Target data source</param>
    internal MutationBuilder(TableauId onTableauId, DataSource dataSource)
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
    public MutationBuilder WithValues(Dictionary<string, object?> valueUpdates)
    {
        _valueUpdates = valueUpdates ?? throw new ArgumentNullException(nameof(valueUpdates));

        (_, string schemaName, string tableauName) = _onTableauId.NameTuple;

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
            if (_dataSource[schemaName][tableauName][referencedAttrName].IsIdentity)
            {
                throw new MutationOnPrimaryKeyNotAllowedException(referencedAttrName);
            }
        }

        _referencedUpdateSet =
            _dataSource[schemaName][tableauName]
                .UpdateSets
                .FirstOrDefault(us => referencedAttrNames.IsSubsetOf(us.AttributeNames));

        if (_referencedUpdateSet is null)
        {
            throw new NoUpdateSetFoundForExpressionAttributesException(referencedAttrNames, _dataSource[schemaName][tableauName]);
        }

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


public sealed class CommandSelectionBuilder
{
    private Option<SelectionExpression> _expression;
    private readonly DataSource _dataSource;
    private readonly TableauId _referencedTableauId;
    private UpdateSet? _referencedUpdateSet;
    internal bool IsConfigured => _expression.IsSome;
    internal UpdateSet? ReferencedUpdateSet => _referencedUpdateSet;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dataSource">Data source on which the query will be executed</param>
    /// <param name="referencedTableauId">Id of the tableau referenced in the command</param>
    internal CommandSelectionBuilder(DataSource dataSource, TableauId referencedTableauId)
    {
        if (referencedTableauId is null)
        {
            throw new ArgumentException($"'{nameof(referencedTableauId)}' cannot be null or empty.", nameof(referencedTableauId));
        }

        _expression = Option<SelectionExpression>.None;
        _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
        _referencedTableauId = referencedTableauId;
    }

    /// <summary>
    /// Creates the selection expression
    /// </summary>
    /// <param name="expression"></param>
    /// <returns>SelectionBuilder</returns>
    public CommandSelectionBuilder WithExpression(SelectionExpression expression)
    {
        if (expression is null)
        {
            throw new ArgumentNullException(nameof(expression));
        }

        (string _, string schemaName, string tableauName) = _referencedTableauId.NameTuple;

        var referencedTableau = _dataSource[schemaName][tableauName];

        var referencableAttrNames = referencedTableau.Attributes.ToDictionary(attr => attr.Id, attr => attr.DataType);

        CommandSelectionUtils.CheckAttributeReferences(expression, referencableAttrNames.Keys.ToHashSet());
        CommandSelectionUtils.CheckAttributeTypesOnComparison(expression, referencableAttrNames ?? new());

        _referencedUpdateSet = CommandSelectionUtils.UpdateSetForExpressionAttributes(expression, referencedTableau);
        if (_referencedUpdateSet is null)
        {
            throw new NoUpdateSetFoundForExpressionAttributesException(
                CommandSelectionUtils.GetReferencedAttributeIds(expression),
                referencedTableau
                );
        }

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