using Janus.Commons.CommandModels.Exceptions;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Commons.QueryModels.Exceptions;
using Janus.Commons.SchemaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.CommandModels;

public interface IPostInitInsertCommandBuilder
{
    IPostInstatiationBuilder WithInstantiation(Func<InstantiationBuilder, InstantiationBuilder> configuration);
}

public interface IPostInstatiationBuilder
{
    InsertCommand Build();
}

public class InsertCommandBuilder : IPostInitInsertCommandBuilder, IPostInstatiationBuilder
{
    private readonly string _onTableauId;
    private readonly DataSource _dataSource;
    private Option<Instantiation> _instantiation;
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

public class InstantiationBuilder
{
    private readonly string _onTableauId;
    private readonly DataSource _dataSource;
    private TabularData? _tableauDataToInsert;

    internal InstantiationBuilder(string onTableauId, DataSource dataSource)
    {
        _onTableauId = onTableauId;
        _dataSource = dataSource;
        _tableauDataToInsert = null;
    }

    public InstantiationBuilder WithValues(TabularData tabularData)
    {
        (_, string schemaName, string tableauName) = Utils.GetNamesFromTableauId(_onTableauId);

        var referencableAttributes = _dataSource[schemaName][tableauName].Attributes.Select(attr => attr.Name).ToHashSet();
        var referencedAttributes = tabularData.AttributeNames;

        if(!referencedAttributes.All(referencableAttributes.Contains))
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
            if(referencedDataType != referencingDataType)
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

    internal Instantiation Build()
    {
        (_, string schemaName, string tableauName) = Utils.GetNamesFromTableauId(_onTableauId);
        return _tableauDataToInsert == null
                ? throw new MissingInstantiationAttributesException(_dataSource[schemaName][tableauName].AttributeNames, _onTableauId)
                : new Instantiation(_tableauDataToInsert);
    }
}
