﻿using FunctionalExtensions.Base.Resulting;
using Janus.Commons.CommandModels;
using Janus.Serialization.Avro.CommandModels.DTOs;
using Janus.Serialization.Avro.DataModels;
using SolTechnology.Avro;

namespace Janus.Serialization.Avro.CommandModels;

/// <summary>
/// Avro format serializer for the insert command
/// </summary>
public sealed class InsertCommandSerializer : ICommandSerializer<InsertCommand, byte[]>
{
    private readonly string _schema = AvroConvert.GenerateSchema(typeof(InsertCommandDto));
    private readonly TabularDataSerializer _tabularDataSerializer = new TabularDataSerializer();

    /// <summary>
    /// Deserializes an insert command
    /// </summary>
    /// <param name="serialized">Serialized insert command</param>
    /// <returns>Deserialized insert command</returns>
    public Result<InsertCommand> Deserialize(byte[] serialized)
        => Results.AsResult(() => AvroConvert.DeserializeHeadless<InsertCommandDto>(serialized, _schema))
            .Bind(FromDto);

    /// <summary>
    /// Serializes an insert command
    /// </summary>
    /// <param name="command">Insert command to serialize</param>
    /// <returns>Serialized insert command</returns>
    public Result<byte[]> Serialize(InsertCommand command)
        => Results.AsResult(()
            => ToDto(command)
                .Map(insertCommandDto => AvroConvert.SerializeHeadless(insertCommandDto, _schema))
        );

    /// <summary>
    /// Converts an insert command to its DTO
    /// </summary>
    /// <param name="insertCommand">Insert command</param>
    /// <returns>Insert command DTO</returns>
    internal Result<InsertCommandDto> ToDto(InsertCommand insertCommand)
        => Results.AsResult(() =>
        {
            var tabularDataDto = _tabularDataSerializer.ToDto(insertCommand.Instantiation.TabularData).Data!;
            var insertCommandDto = new InsertCommandDto
            {
                OnTableauId = insertCommand.OnTableauId,
                Instantiation = tabularDataDto
            };

            return insertCommandDto;
        });

    /// <summary>
    /// Converts an insert command DTO to the command model
    /// </summary>
    /// <param name="insertCommandDto">Insert command DTO</param>
    /// <returns>Insert command model</returns>
    internal Result<InsertCommand> FromDto(InsertCommandDto insertCommandDto)
        => Results.AsResult(() =>
        {
            var tabularData = _tabularDataSerializer.FromDto(insertCommandDto.Instantiation).Data!;

            var insertCommand =
            InsertCommandOpenBuilder.InitOpenInsert(insertCommandDto.OnTableauId)
                .WithInstantiation(conf => conf.WithValues(tabularData))
                .Build();

            return insertCommand;
        });
}
