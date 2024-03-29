﻿using Janus.Commons.SelectionExpressions;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.Serialization.Json.QueryModels.DTOs;

internal class SelectionDto
{
    public SelectionExpression Expression { get; set; } = TRUE();
}
