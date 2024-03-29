﻿namespace Janus.Serialization.Bson.SchemaModels.DTOs;
/// <summary>
/// DTO representation of an UpdateSet
/// </summary>
internal sealed class UpdateSetDto
{
    public HashSet<string> AttributeIds { get; set; } = new HashSet<string>();
}
