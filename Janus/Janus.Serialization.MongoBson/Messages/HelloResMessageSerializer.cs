﻿using Janus.Base.Resulting;
using Janus.Commons.Messages;
using Janus.Serialization.MongoBson.Messages.DTOs;

namespace Janus.Serialization.MongoBson.Messages;

/// <summary>
/// MongoBson format HELLO_RES message serializer
/// </summary>
public sealed class HelloResMessageSerializer : IMessageSerializer<HelloResMessage, byte[]>
{

    /// <summary>
    /// Deserializes a HELLO_RES message
    /// </summary>
    /// <param name="serialized">Serialized HELLO_RES</param>
    /// <returns>Deserialized HELLO_RES</returns>
    public Result<HelloResMessage> Deserialize(byte[] serialized)
        => Results.AsResult(() =>
        {
            var deserializedModel = Utils.FromBson<HelloResMessageDto>(serialized);

            return new HelloResMessage(
                deserializedModel.ExchangeId,
                deserializedModel.NodeId,
                deserializedModel.ListenPort,
                deserializedModel.NodeType,
                deserializedModel.RememberMe,
                deserializedModel.ContextMessage);
        });

    /// <summary>
    /// Serializes a HELLO_RES message
    /// </summary>
    /// <param name="message">HELLO_RES message to serialize</param>
    /// <returns>Serialized HELLO_RES</returns>
    public Result<byte[]> Serialize(HelloResMessage message)
        => Results.AsResult(() =>
        {
            var helloResMessageDto = new HelloResMessageDto
            {
                Preamble = message.Preamble,
                ExchangeId = message.ExchangeId,
                NodeId = message.NodeId,
                ListenPort = message.ListenPort,
                NodeType = message.NodeType,
                RememberMe = message.RememberMe,
                ContextMessage = message.ContextMessage
            };
            return Utils.ToBson(helloResMessageDto);
        });
}
