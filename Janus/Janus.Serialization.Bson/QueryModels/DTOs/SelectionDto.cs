using Janus.Commons.SelectionExpressions;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.Serialization.Bson.QueryModels.DTOs;

internal sealed class SelectionDto
{
    public SelectionExpression Expression { get; set; } = TRUE();
}
