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
public class InsertCommandBuilder : IPostInitInsertCommandBuilder, IPostInstatiationBuilder
{
    private readonly string _onTableauId;
    private readonly DataSource _dataSource;
    private Option<Instantiation> _instantiation;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="onTableauId">Starting tableau</param>
    /// <param name="dataSource">Command's Target data source</param>
    internal InsertCommandBuilder(string onTableauId, DataSource dataSource)
    {
        _onTableauId = onTableauId;
        _dataSource = dataSource;
        _instantiation = Option<Instantiation>.None;
    }

    public InsertCommand Build()
        => _instantiation.IsSome
           ? new InsertCommand(_onTableauId, _instantiation.Value)
           : throw new InstantiationNotSetException();

    /// <summary>
    /// Initializes a insert command builder on a tableau from a data source
    /// </summary>
    /// <param name="onTableauId">Starting tableau</param>
    /// <param name="dataSource">Command's target data source</param>
    /// <returns></returns>
    /// <exception cref="TableauDoesNotExistException"></exception>
    public static IPostInitInsertCommandBuilder InitOnDataSource(string onTableauId, DataSource dataSource)
    {
        if (!dataSource.ContainsTableau(onTableauId))
        {
            throw new TableauDoesNotExistException(onTableauId, dataSource.Name);
        }
        return new InsertCommandBuilder(onTableauId, dataSource);
    }

    public IPostInstatiationBuilder WithInstantiation(Func<InstantiationBuilder, InstantiationBuilder> configuration)
    {
        var instantiationBuilder = new InstantiationBuilder(_onTableauId, _dataSource);
        _instantiation = Option<Instantiation>.Some(configuration(instantiationBuilder).Build());

        return this;
    }
}

/// <summary>
/// Builder for the instantiation clause
/// </summary>
public class InstantiationBuilder
{
    private readonly string _onTableauId;
    private readonly DataSource _dataSource;
    private TabularData? _tableauDataToInsert;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="onTableauId">Starting tableau</param>
    /// <param name="dataSource">Target data source</param>
    internal InstantiationBuilder(string onTableauId, DataSource dataSource)
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
        (_, string schemaName, string tableauName) = Utils.GetNamesFromTableauId(_onTableauId);

        var referencableAttributes = _dataSource[schemaName][tableauName].Attributes.Select(attr => attr.Name).ToHashSet();
        var referencedAttributes = tabularData.AttributeNames;

        if (!referencedAttributes.All(referencableAttributes.Contains))
        {
            var invalidAttrRef = referencedAttributes.Where(referencedAttr => !referencableAttributes.Contains(referencedAttr)).First();
            throw new AttributeNotInTargetTableauException(invalidAttrRef, _onTableauId);
        }
        if (!referencableAttributes.All(referencedAttributes.Contains))
        {
            var unreferencedAttrs = referencableAttributes.Except(referencedAttributes).ToList();
            throw new MissingInstantiationAttributesException(unreferencedAttrs, _onTableauId);
        }

        foreach (var referencedAttr in referencedAttributes)
        {
            var referencedDataType = _dataSource[schemaName][tableauName][referencedAttr].DataType;
            var referencingDataType = tabularData.AttributeDataTypes[referencedAttr];
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

        return this;
    }

    /// <summary>
    /// Builds the instantiation clause
    /// </summary>
    /// <returns></returns>
    /// <exception cref="MissingInstantiationAttributesException"></exception>
    internal Instantiation Build()
    {
        (_, string schemaName, string tableauName) = Utils.GetNamesFromTableauId(_onTableauId);
        return _tableauDataToInsert == null
                ? throw new MissingInstantiationAttributesException(_dataSource[schemaName][tableauName].AttributeNames, _onTableauId)
                : new Instantiation(_tableauDataToInsert);
    }
}
