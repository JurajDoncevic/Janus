﻿namespace Janus.Serialization.Bson.QueryModels.DTOs;

internal sealed class JoinDto
{
    public string PrimaryKeyAttributeId { get; set; } = string.Empty;
    public string ForeignKeyAttributeId { get; set; } = string.Empty;
}
