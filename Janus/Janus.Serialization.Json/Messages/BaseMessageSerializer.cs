﻿using Janus.Base.Resulting;
using Janus.Commons.Messages;
using System.Text.Json;

namespace Janus.Serialization.Json.Messages;

/// <summary>
/// JSON base message serializer
/// </summary>
public class BaseMessageSerializer : IMessageSerializer<BaseMessage, string>
{
    /// <summary>
    /// Deserializes a base message
    /// </summary>
    /// <param name="serialized">Serialized base message</param>
    /// <returns>Deserialized base message</returns>
    public Result<BaseMessage> Deserialize(string serialized)
        => Results.AsResult(() => JsonSerializer.Deserialize<BaseMessage>(serialized) ?? throw new Exception("Failed to deserialize message DTO"));

    /// <summary>
    /// Serializes a base message
    /// </summary>
    /// <param name="message">Base message to serialize</param>
    /// <returns>Serialized base message</returns>
    public Result<string> Serialize(BaseMessage message)
        => Results.AsResult(() => JsonSerializer.Serialize(message));
}
