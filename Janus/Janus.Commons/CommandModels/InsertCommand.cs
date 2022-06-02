using Janus.Commons.CommandModels.Exceptions;
using Janus.Commons.CommandModels.JsonConversion;
using Janus.Commons.QueryModels.Exceptions;
using Janus.Commons.SchemaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Janus.Commons.CommandModels;

[JsonConverter(typeof(InsertCommandJsonConverter))]
public class InsertCommand : BaseCommand
{

    private readonly Instantiation _instantiation;

    internal InsertCommand(string onTableauId!!, Instantiation instantiation!!) : base(onTableauId)
    {
        _instantiation = instantiation;
    }

    public Instantiation Instantiation => _instantiation;

    public Result IsValidForDataSource(DataSource dataSource)
        => ResultExtensions.AsResult(() =>
        {
            if (!dataSource.ContainsTableau(_onTableauId))
            {
                throw new TableauDoesNotExistException(_onTableauId, dataSource.Name);
            }

            (_, string schemaName, string tableauName) = Utils.GetNamesFromTableauId(_onTableauId);

            var referencableAttributes = dataSource[schemaName][tableauName].Attributes.Select(attr => attr.Name).ToHashSet();
            var referencedAttributes = _instantiation.TabularData.AttributeNames;

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
                var referencedDataType = dataSource[schemaName][tableauName][referencedAttr].DataType;
                var referencingDataType = _instantiation.TabularData.AttributeDataTypes[referencedAttr];
                if (referencedDataType != referencingDataType)
                {
                    throw new IncompatibleInstantiationDataTypesException(_onTableauId, referencedAttr, referencedDataType, referencingDataType);
                }
            }

            foreach (var referencedAttr in referencableAttributes)
            {
                if (!dataSource[schemaName][tableauName][referencedAttr].IsNullable)
                {
                    foreach (var row in _instantiation.TabularData.RowData)
                    {
                        if (row[referencedAttr] == null)
                            throw new NullGivenForNonNullableAttributeException(_onTableauId, referencedAttr);
                    }
                }
            }

            return true;
        });

    public override bool Equals(object? obj)
    {
        return obj is InsertCommand command &&
               _onTableauId.Equals(command._onTableauId) &&
               _instantiation.Equals(command._instantiation);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_onTableauId, _instantiation);
    }
}
