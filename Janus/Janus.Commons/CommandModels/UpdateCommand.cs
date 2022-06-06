using Janus.Commons.CommandModels.Exceptions;
using Janus.Commons.CommandModels.JsonConversion;
using Janus.Commons.DataModels;
using Janus.Commons.SchemaModels;
using Janus.Commons.SelectionExpressions;
using static Janus.Commons.SelectionExpressions.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Janus.Commons.CommandModels;

/// <summary>
/// Describes an update command
/// </summary>
[JsonConverter(typeof(UpdateCommandJsonConverter))]
public class UpdateCommand : BaseCommand
{
    private readonly Mutation _mutation;
    private readonly Option<CommandSelection> _selection;

    /// <summary>
    /// Mutation clause
    /// </summary>
    public Mutation Mutation => _mutation;

    /// <summary>
    /// Selection clause
    /// </summary>
    public Option<CommandSelection> Selection => _selection;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="onTableauId">Starting tableau</param>
    /// <param name="mutation">Mutation clause</param>
    /// <param name="selection">Selection clause</param>
    internal UpdateCommand(string onTableauId!!, Mutation mutation!!, Option<CommandSelection> selection) : base(onTableauId)
    {
        _mutation = mutation;
        _selection = selection;
    }

    public override Result IsValidForDataSource(DataSource dataSource)
        => ResultExtensions.AsResult(() =>
        {
            UpdateCommandBuilder.InitOnDataSource(_onTableauId, dataSource)
                .WithMutation(configuration => configuration.WithValues(_mutation.ValueUpdates.ToDictionary(kv => kv.Key, kv => kv.Value)))
                .WithSelection(configuration =>
                    _selection.Match(
                        selection => configuration.WithExpression(selection.Expression),
                        () => configuration
                        ))
                .Build();

            return true;
        });

    public override bool Equals(object? obj)
    {
        return obj is UpdateCommand command &&
               _onTableauId == command._onTableauId &&
               EqualityComparer<Mutation>.Default.Equals(_mutation, command._mutation) &&
               EqualityComparer<Option<CommandSelection>>.Default.Equals(_selection, command._selection);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_onTableauId, _mutation, _selection);
    }

}
