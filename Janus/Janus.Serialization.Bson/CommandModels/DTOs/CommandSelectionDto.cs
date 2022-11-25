using Janus.Commons.SelectionExpressions;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.Serialization.Bson.CommandModels.DTOs;

public sealed class CommandSelectionDto
{
    public SelectionExpression SelectionExpression { get; set; } = FALSE();
}
