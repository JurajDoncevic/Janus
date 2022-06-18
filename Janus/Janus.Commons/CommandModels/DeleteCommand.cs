﻿using Janus.Commons.CommandModels.JsonConversion;
using Janus.Commons.SchemaModels;
using System.Text.Json.Serialization;

namespace Janus.Commons.CommandModels;

/// <summary>
/// Describes a delete command
/// </summary>
[JsonConverter(typeof(DeleteCommandJsonConverter))]
public class DeleteCommand : BaseCommand
{
    private readonly Option<CommandSelection> _selection;

    /// <summary>
    /// Selection clause
    /// </summary>
    public Option<CommandSelection> Selection => _selection;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="onTableauId">Starting tableau</param>
    /// <param name="selection">Selection clause</param>
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
            DeleteCommandBuilder.InitOnDataSource(_onTableauId, dataSource)
                .WithSelection(conf =>
                    _selection.Match(
                        selection => conf.WithExpression(selection.Expression),
                        () => conf
                    ))
                .Build();
            return true;
        });
}
