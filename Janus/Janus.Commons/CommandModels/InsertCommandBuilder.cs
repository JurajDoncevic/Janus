using Janus.Commons.CommandModels.Exceptions;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels.Exceptions;
using Janus.Commons.SchemaModels;

namespace Janus.Commons.CommandModels;

#region BUILDER SEQUENCE INTERFACES
public interface IPostInitInsertCommandBuilder
{
    /// <summary>
    /// Sets the instantiation clause
    /// </summary>
    /// <param name="configuration">Instantiation configuration</param>
    /// <returns></returns>
    IPostInstatiationBuilder WithInstantiation(Func<InstantiationBuilder, InstantiationBuilder> configuration);
    /// <summary>
    /// Sets the insert command name
    /// </summary>
    /// <param name="name">Command name</param>
    /// <returns></returns>
    IPostInitInsertCommandBuilder WithName(string name);
}

public interface IPostInstatiationBuilder
{
    /// <summary>
    /// Builds the specified insert command
    /// </summary>
    /// <returns></returns>
    InsertCommand Build();
}
#endregion

/// <summary>
/// Builder for the insert command
/// </summary>
public sealed class InsertCommandBuilder : IPostInitInsertCommandBuilder, IPostInstatiationBuilder
{
    private readonly TableauId _onTableauId;
    private readonly DataSource _dataSource;
    private Option<Instantiation> _instantiation;
    private string _commandName;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="onTableauId">Starting tableau</param>
    /// <param name="dataSource">Command's Target data source</param>
    internal InsertCommandBuilder(TableauId onTableauId, DataSource dataSource)
    {
        _onTableauId = onTableauId;
        _dataSource = dataSource;
        _instantiation = Option<Instantiation>.None;
        _commandName = Guid.NewGuid().ToString(); // pre-empt null
    }

    public InsertCommand Build()
        => _instantiation.IsSome
           ? new InsertCommand(_onTableauId, _instantiation.Value, _commandName)
           : throw new InstantiationNotSetException();

    /// <summary>
    /// Initializes a insert command builder on a tableau from a data source
    /// </summary>
    /// <param name="onTableauId">Starting tableau</param>
    /// <param name="dataSource">Command's target data source</param>
    /// <returns></returns>
    /// <exception cref="TableauDoesNotExistException"></exception>
    public static IPostInitInsertCommandBuilder InitOnDataSource(TableauId onTableauId, DataSource dataSource)
    {
        if (!dataSource.ContainsTableau(onTableauId))
        {
            throw new TableauDoesNotExistException(onTableauId, dataSource.Name);
        }

        (string _, string schemaName, string tableauName) = onTableauId.NameTuple;

        if (!dataSource[schemaName][tableauName].UpdateSets.Any(us => us.AttributeNames.SequenceEqual(dataSource[schemaName][tableauName].AttributeNames)))
            throw new CommandAllowedOnTableauWideUpdateSetException();

        return new InsertCommandBuilder(onTableauId, dataSource);
    }

    /// <summary>
    /// Initializes a insert command builder on a tableau from a data source
    /// </summary>
    /// <param name="onTableauId">Starting tableau</param>
    /// <param name="dataSource">Command's target data source</param>
    /// <returns></returns>
    /// <exception cref="TableauDoesNotExistException"></exception>
    public static IPostInitInsertCommandBuilder InitOnDataSource(string onTableauId, DataSource dataSource)
        => InitOnDataSource(TableauId.From(onTableauId), dataSource);

    public IPostInstatiationBuilder WithInstantiation(Func<InstantiationBuilder, InstantiationBuilder> configuration)
    {
        var instantiationBuilder = new InstantiationBuilder(_onTableauId, _dataSource);
        _instantiation = Option<Instantiation>.Some(configuration(instantiationBuilder).Build());

        return this;
    }

    public IPostInitInsertCommandBuilder WithName(string name)
    {
        _commandName = name ?? _commandName;

        return this;
    }
}

/// <summary>
/// Builder for the instantiation clause
/// </summary>
public sealed class InstantiationBuilder
{
    private readonly TableauId _onTableauId;
    private readonly DataSource _dataSource;
    private TabularData? _tableauDataToInsert;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="onTableauId">Starting tableau</param>
    /// <param name="dataSource">Target data source</param>
    internal InstantiationBuilder(TableauId onTableauId, DataSource dataSource)
    {
        _onTableauId = onTableauId;
        _dataSource = dataSource;
        _tableauDataToInsert = null;
    }

    /// <summary>
    /// Sets the values in the instantiation clause
    /// </summary>
    /// <param name="tabularData">Data to be inserted. <b>The attributes are qualified by their names.</b> Don't use attribute ids</param>
    /// <returns></returns>
    /// <exception cref="AttributeNotInTargetTableauException"></exception>
    /// <exception cref="MissingInstantiationAttributesException"></exception>
    /// <exception cref="IncompatibleInstantiationDataTypesException"></exception>
    /// <exception cref="NullGivenForNonNullableAttributeException"></exception>
    public InstantiationBuilder WithValues(TabularData tabularData)
    {
        (_, string schemaName, string tableauName) = _onTableauId.NameTuple;

        var referencableAttributes = _dataSource[schemaName][tableauName].Attributes.Select(attr => attr.Name).ToHashSet();
        var referencedAttributeNames = tabularData.ColumnNames;

        if (!referencedAttributeNames.All(referencableAttributes.Contains))
        {
            var invalidAttrRef = referencedAttributeNames.Where(referencedAttr => !referencableAttributes.Contains(referencedAttr)).First();
            throw new AttributeNotInTargetTableauException(invalidAttrRef, _onTableauId);
        }
        if (!referencableAttributes.All(referencedAttributeNames.Contains))
        {
            var unreferencedAttrs = referencableAttributes.Except(referencedAttributeNames).ToList();
            throw new MissingInstantiationAttributesException(unreferencedAttrs, _onTableauId);
        }

        foreach (var referencedAttr in referencedAttributeNames)
        {
            var referencedDataType = _dataSource[schemaName][tableauName][referencedAttr].DataType;
            var referencingDataType = tabularData.ColumnDataTypes[referencedAttr];
            if (referencedDataType != referencingDataType)
            {
                throw new IncompatibleInstantiationDataTypesException(_onTableauId, referencedAttr, referencedDataType, referencingDataType);
            }
        }
        foreach (var referencedAttr in referencableAttributes)
        {
            if (!_dataSource[schemaName][tableauName][referencedAttr].IsNullable)
            {
                foreach (var row in tabularData.RowData)
                {
                    if (row[referencedAttr] == null)
                        throw new NullGivenForNonNullableAttributeException(_onTableauId, referencedAttr);
                }
            }
        }
        _tableauDataToInsert = tabularData;

        var existsValidUpdateSet =
            _dataSource[schemaName][tableauName]
                .UpdateSets
                .Any(us => referencedAttributeNames.IsSubsetOf(us.AttributeNames));

        if (!existsValidUpdateSet)
        {
            throw new NoUpdateSetFoundForExpressionAttributesException(referencedAttributeNames, _dataSource[schemaName][tableauName]);
        }

        return this;
    }

    /// <summary>
    /// Builds the instantiation clause
    /// </summary>
    /// <returns></returns>
    /// <exception cref="MissingInstantiationAttributesException"></exception>
    internal Instantiation Build()
    {
        (_, string schemaName, string tableauName) = _onTableauId.NameTuple;
        return _tableauDataToInsert == null
                ? throw new MissingInstantiationAttributesException(_dataSource[schemaName][tableauName].AttributeNames, _onTableauId)
                : new Instantiation(_tableauDataToInsert);
    }
}
