using Janus.Commons.SelectionExpressions;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.Serialization.MongoBson.QueryModels.DTOs;

internal class SelectionDto
{
    public string Expression { get; set; } = "TRUE";
}
