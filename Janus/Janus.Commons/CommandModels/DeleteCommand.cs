using Janus.Commons.SelectionExpressions;
using static Janus.Commons.SelectionExpressions.SelectionExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Janus.Commons.SchemaModels;
using Janus.Commons.QueryModels.Exceptions;
using System.Text.Json.Serialization;
using Janus.Commons.CommandModels.JsonConversion;

namespace Janus.Commons.CommandModels;

/// <summary>
/// Describes a DELETE command
/// </summary>
[JsonConverter(typeof(DeleteCommandJsonConverter))]
public class DeleteCommand : BaseCommand
{
    private readonly Option<CommandSelection> _selection;

    public Option<CommandSelection> Selection => _selection;

    internal DeleteCommand(string onTableauId, Option<CommandSelection> selection) : base(onTableauId)
    {
        _selection = selection;
    }

    public override bool Equals(object? obj)
    {
        return obj is DeleteCommand command &&
               _onTableauId.Equals(command._onTableauId) &&
               _selection.Equals(command._selection);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_onTableauId, _selection);
    }

    public override Result IsValidForDataSource(DataSource dataSource)
        => ResultExtensions.AsResult(() =>
        {
            (_, string schemaName, string tableauName) = Utils.GetNamesFromTableauId(_onTableauId);

            if (!dataSource.ContainsTableau(_onTableauId))
                throw new TableauDoesNotExistException(_onTableauId, dataSource.Name);

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
