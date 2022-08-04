﻿using FunctionalExtensions.Base.Results;
using Janus.Commons.Messages;
using Janus.Serialization.Avro.Messages.DTOs;
using SolTechnology.Avro;

namespace Janus.Serialization.Avro.Messages;

/// <summary>
/// Avro format HELLO_RES message serializer
/// </summary>
public class HelloResMessageSerializer : IMessageSerializer<HelloResMessage, byte[]>
{
    private readonly string _schema = AvroConvert.GenerateSchema(typeof(HelloResMessageDto));

    /// <summary>
    /// Deserializes a HELLO_RES message
    /// </summary>
    /// <param name="serialized">Serialized HELLO_RES</param>
    /// <returns>Deserialized HELLO_RES</returns>
    public Result<HelloResMessage> Deserialize(byte[] serialized)
        => ResultExtensions.AsResult(() =>
        {
            var deserializedModel = AvroConvert.DeserializeHeadless<HelloResMessageDto>(serialized, _schema);

            return new HelloResMessage(
                deserializedModel.ExchangeId,
                deserializedModel.NodeId,
                deserializedModel.ListenPort,
                deserializedModel.NodeType,
                deserializedModel.RememberMe);
        });

    /// <summary>
    /// Serializes a HELLO_RES message
    /// </summary>
    /// <param name="message">HELLO_RES message to serialize</param>
    /// <returns>Serialized HELLO_RES</returns>
    public Result<byte[]> Serialize(HelloResMessage message)
        => ResultExtensions.AsResult(() =>
        {
            var helloResMessageDto = new HelloResMessageDto
            {
                Preamble = message.Preamble,
                ExchangeId = message.ExchangeId,
                NodeId = message.NodeId,
                ListenPort = message.ListenPort,
                NodeType = message.NodeType,
                RememberMe = message.RememberMe
            };
            return AvroConvert.SerializeHeadless(helloResMessageDto, _schema);
        });
}