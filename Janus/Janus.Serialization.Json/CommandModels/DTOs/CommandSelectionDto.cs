﻿using Janus.Commons.SelectionExpressions;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.Serialization.Json.CommandModels.DTOs;

public class CommandSelectionDto
{
    public SelectionExpression SelectionExpression { get; set; } = FALSE();
}
