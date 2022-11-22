using FunctionalExtensions.Base.Resulting;
using Janus.Commons.CommandModels;
using Janus.Serialization.Bson.CommandModels.DTOs;
using Janus.Serialization.Bson.DataModels;
using Janus.Serialization.Bson.QueryModels;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Janus.Serialization.Bson.CommandModels;

/// <summary>
/// BSON format serializer for the insert command
/// </summary>
public class InsertCommandSerializer : ICommandSerializer<InsertCommand, byte[]>
{
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly TabularDataSerializer _tabularDataSerializer = new TabularDataSerializer();

    public InsertCommandSerializer()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new SelectionExpressionJsonConverter());
        options.Converters.Add(new JsonStringEnumConverter());

        _serializerOptions = options;
    }

    /// <summary>
    /// Deserializes an insert command
    /// </summary>
    /// <param name="serialized">Serialized insert command</param>
    /// <returns>Deserialized insert command</returns>
    public Result<InsertCommand> Deserialize(byte[] serialized)
        => Results.AsResult(() => JsonSerializer.Deserialize<InsertCommandDto>(serialized, _serializerOptions) ?? throw new Exception("Failed to deserialize InsertCommandDTO"))
            .Bind(FromDto);

    /// <summary>
    /// Serializes an insert command
    /// </summary>
    /// <param name="command">Insert command to serialize</param>
    /// <returns>Serialized insert command</returns>
    public Result<byte[]> Serialize(InsertCommand command)
        => Results.AsResult(()
            => ToDto(command)
                .Map(insertDto => JsonSerializer.Serialize(insertDto, _serializerOptions))
                .Map(Encoding.UTF8.GetBytes));

    /// <summary>
    /// Converts an insert command to its DTO
    /// </summary>
    /// <param name="insertCommand">Insert command</param>
    /// <returns>Insert command DTO</returns>
    internal Result<InsertCommandDto> ToDto(InsertCommand insertCommand)
        => Results.AsResult(() =>
        {
            var tabularData = insertCommand.Instantiation.TabularData;
            var insertDto = new InsertCommandDto()
            {
                OnTableauId = insertCommand.OnTableauId,
                Instantiation = _tabularDataSerializer.ToDto(tabularData).Data!
            };

            return insertDto;
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

            var insertCommand = InsertCommandOpenBuilder.InitOpenInsert(insertCommandDto.OnTableauId)
                                    .WithInstantiation(conf => conf.WithValues(tabularData))
                                    .Build();

            return insertCommand;
        });
}
