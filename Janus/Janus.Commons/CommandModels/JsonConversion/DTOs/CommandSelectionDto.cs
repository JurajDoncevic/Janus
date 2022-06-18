using Janus.Commons.SelectionExpressions;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.Commons.CommandModels.JsonConversion.DTOs;

public class CommandSelectionDto
{
    public SelectionExpression SelectionExpression { get; set; } = FALSE();
}
