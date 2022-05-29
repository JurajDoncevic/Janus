using Janus.Commons.CommandModels.Exceptions;
using Janus.Commons.DataModels;
using Janus.Commons.SchemaModels;
using Janus.Commons.SelectionExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.CommandModels;

public class UpdateCommand
{
    private readonly string _onTableauId;
    private readonly Mutation _mutation;
    private readonly Option<CommandSelection> _selection;

    internal UpdateCommand(string onTableauId!!, Mutation mutation!!, Option<CommandSelection> selection)
    {
        _onTableauId = onTableauId;
        _mutation = mutation;
        _selection = selection;
    }

    public string OnTableauId => _onTableauId;

    public Mutation Mutation => _mutation;

    public Option<CommandSelection> Selection => _selection;

    public Result IsValidOnDataSource(DataSource dataSource)
        => ResultExtensions.AsResult(() =>
        {
            (_, string schemaName, string tableauName) = Utils.GetNamesFromTableauId(_onTableauId);

            var referencedAttrs = _mutation.ValueUpdates.Keys.ToHashSet();
            var referencableAttrs = dataSource[schemaName][tableauName].AttributeNames.ToHashSet();

            if (!referencedAttrs.All(referencableAttrs.Contains))
            {
                var invalidAttrRef = referencedAttrs.Where(referencedAttr => !referencableAttrs.Contains(referencedAttr)).First();
                throw new AttributeNotInTargetTableauException(invalidAttrRef, _onTableauId);
            }

            foreach (var referencedAttr in referencedAttrs)
            {
                if (_mutation.ValueUpdates[referencedAttr] != null) // can't get null type
                {
                    var referencedDataType = dataSource[schemaName][tableauName][referencedAttr].DataType;
                    var referencingDataType = TypeMappings.MapToDataType(_mutation.ValueUpdates[referencedAttr].GetType());
                    if (referencedDataType != referencingDataType)
                    {
                        throw new IncompatibleMutationDataTypesException(_onTableauId, referencedAttr, referencedDataType, referencingDataType);
                    }
                }
            }

            foreach (var referencedAttr in referencableAttrs.Intersect(_mutation.ValueUpdates.Keys))
            {
                if (!dataSource[schemaName][tableauName][referencedAttr].IsNullable && _mutation.ValueUpdates[referencedAttr] == null)
                    throw new NullGivenForNonNullableAttributeException(_onTableauId, referencedAttr);
            }

            if (_selection)
            {
                var selectionExpression = _selection.Value.Expression;

                var referencableAttrsBySelection = dataSource[schemaName][tableauName].Attributes.Map(a => (a.Name, a.DataType))
                                 .ToDictionary(x => x.Name, x => x.DataType);

                CommandSelectionUtils.CheckAttributeReferences(selectionExpression, referencableAttrsBySelection.Keys.ToHashSet());
                CommandSelectionUtils.CheckAttributeTypesOnComparison(selectionExpression, referencableAttrsBySelection);
            }
            return true;
        });
}
