using Janus.Commons.CommandModels.Exceptions;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Commons.QueryModels.Exceptions;
using Janus.Commons.SchemaModels;
using Janus.Commons.SelectionExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.CommandModels;

public interface IPostInitUpdateCommandBuilder
{
    IPostMutationUpdateCommandBuilder WithMutation(Func<MutationBuilder, MutationBuilder> configuration);
}

public interface IPostMutationUpdateCommandBuilder
{
    IPostSelectionUpdateCommandBuilder WithSelection(Func<CommandSelectionBuilder, CommandSelectionBuilder> configuration);
    UpdateCommand Build();
}

public interface IPostSelectionUpdateCommandBuilder
{
    UpdateCommand Build();
}

public class UpdateCommandBuilder : IPostInitUpdateCommandBuilder, IPostMutationUpdateCommandBuilder, IPostSelectionUpdateCommandBuilder
{
    private readonly string _onTableauId;
    private readonly DataSource _dataSource;
    private Mutation? _mutation;
    private CommandSelection? _selection;

    internal UpdateCommandBuilder(string onTableauId, DataSource dataSource)
    {
        _onTableauId = onTableauId;
        _dataSource = dataSource;
    }

    public static IPostInitUpdateCommandBuilder InitOnDataSource(string onTableauId, DataSource dataSource)
    {
        if (!dataSource.ContainsTableau(onTableauId))
            throw new TableauDoesNotExistException(onTableauId, dataSource.Name);

        return new UpdateCommandBuilder(onTableauId, dataSource);
    }

    public IPostMutationUpdateCommandBuilder WithMutation(Func<MutationBuilder, MutationBuilder> configuration)
    {
        var mutationBuilder = new MutationBuilder(_onTableauId, _dataSource);

        _mutation = configuration(mutationBuilder).Build();

        return this;
    }

    public IPostSelectionUpdateCommandBuilder WithSelection(Func<CommandSelectionBuilder, CommandSelectionBuilder> configuration)
    {
        var selectionBuilder = new CommandSelectionBuilder(_dataSource, new() { _onTableauId });
        _selection = configuration(selectionBuilder).Build();

        return this;
    }

    public UpdateCommand Build()
    {
        if (_mutation == null)
            throw new MutationNotSetException();
        return new UpdateCommand(_onTableauId,
                                 _mutation,
                                 _selection == null ? Option<CommandSelection>.None : Option<CommandSelection>.Some(_selection));

    }
}

public class MutationBuilder
{
    private Dictionary<string, object>? _valueUpdates;
    private readonly string _onTableauId;
    private readonly DataSource _dataSource;

    internal MutationBuilder(string onTableauId, DataSource dataSource)
    {
        _onTableauId = onTableauId;
        _dataSource = dataSource;
    }

    public MutationBuilder WithValues(Dictionary<string, object> valueUpdates)
    {
        (_, string schemaName, string tableauName) = Utils.GetNamesFromTableauId(_onTableauId);

        var referencedAttrs = valueUpdates.Keys.ToHashSet();
        var referencableAttrs = _dataSource[schemaName][tableauName].AttributeNames.ToHashSet();

        if (!referencedAttrs.All(referencableAttrs.Contains))
        {
            var invalidAttrRef = referencedAttrs.Where(referencedAttr => !referencableAttrs.Contains(referencedAttr)).First();
            throw new AttributeNotInTargetTableauException(invalidAttrRef, _onTableauId);
        }

        foreach (var referencedAttr in referencedAttrs)
        {
            if (valueUpdates[referencedAttr] != null) // can't get null type
            {
                var referencedDataType = _dataSource[schemaName][tableauName][referencedAttr].DataType;
                var referencingDataType = TypeMappings.MapToDataType(valueUpdates[referencedAttr].GetType());
                if (referencedDataType != referencingDataType)
                {
                    throw new IncompatibleMutationDataTypesException(_onTableauId, referencedAttr, referencedDataType, referencingDataType);
                }
            }
        }

        foreach (var referencedAttr in referencableAttrs.Intersect(valueUpdates.Keys))
        {
                if (!_dataSource[schemaName][tableauName][referencedAttr].IsNullable && valueUpdates[referencedAttr] == null)
                    throw new NullGivenForNonNullableAttributeException(_onTableauId, referencedAttr);
        }

        _valueUpdates = valueUpdates;
        return this;
    }

    internal Mutation Build()
        => _valueUpdates != null
           ? new Mutation(_valueUpdates)
           : new Mutation(new());
}


public class CommandSelectionBuilder
{
    private SelectionExpression? _expression;
    private readonly DataSource _dataSource;
    private readonly HashSet<string> _referencedTableauIds;
    internal bool IsConfigured => _expression != null;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dataSource">Data source on which the query will be executed</param>
    /// <param name="referencedTableauIds">Ids of tableaus referenced in the query</param>
    internal CommandSelectionBuilder(DataSource dataSource!!, HashSet<string> referencedTableauIds!!)
    {
        _expression = null;
        _dataSource = dataSource;
        _referencedTableauIds = referencedTableauIds;
    }

    /// <summary>
    /// Creates the selection expression
    /// </summary>
    /// <param name="expression"></param>
    /// <returns>SelectionBuilder</returns>
    public CommandSelectionBuilder WithExpression(SelectionExpression expression)
    {
        var referencableAttrNames = _referencedTableauIds.Select(tId => Utils.GetNamesFromTableauId(tId))
                                                     .SelectMany(names => _dataSource[names.schemaName][names.tableauName].Attributes.Map(a => (a.Name, a.DataType)))
                                                     .ToDictionary(x => x.Name, x => x.DataType);

        CommandSelectionUtils.CheckAttributeReferences(expression, referencableAttrNames.Keys.ToHashSet());
        CommandSelectionUtils.CheckAttributeTypesOnComparison(expression, referencableAttrNames ?? new());


        _expression = expression;
        return this;
    }

    /// <summary>
    /// Builds the specified selection
    /// </summary>
    /// <returns></returns>
    internal CommandSelection Build()
    {
        return new CommandSelection(_expression ?? new TrueLiteral());
    }
}