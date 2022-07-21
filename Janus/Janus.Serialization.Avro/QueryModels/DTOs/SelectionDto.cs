using Janus.Commons.SelectionExpressions;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.Serialization.Avro.QueryModels.DTOs;

internal class SelectionDto
{
    public string Expression { get; set; } = "TRUE";
}
