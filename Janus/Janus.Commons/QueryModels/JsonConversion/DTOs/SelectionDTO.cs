using Janus.Commons.SelectionExpressions;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.Commons.QueryModels.JsonConversion.DTOs;

public class SelectionDTO
{
    public SelectionExpression Expression { get; set; } = TRUE();
}
