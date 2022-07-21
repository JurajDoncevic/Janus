using FunctionalExtensions.Base.Results;
using Janus.Commons.Messages;
using Janus.Serialization.Avro.Messages.DTOs;
using SolTechnology.Avro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return AvroConvert.SerializeHeadless(message, _schema);
        });
}
