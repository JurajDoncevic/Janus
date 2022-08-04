using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Results;
using Janus.Commons.CommandModels;
using Janus.Serialization.Json.CommandModels.DTOs;
using Janus.Serialization.Json.DataModels;
using Janus.Serialization.Json.QueryModels;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Janus.Serialization.Json.CommandModels;

/// <summary>
/// JSON format serializer for the insert command
/// </summary>
public class InsertCommandSerializer : ICommandSerializer<InsertCommand, string>
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
    public Result<InsertCommand> Deserialize(string serialized)
        => ResultExtensions.AsResult(() => JsonSerializer.Deserialize<InsertCommandDto>(serialized, _serializerOptions) ?? throw new Exception("Failed to serialize message DTO"))
            .Bind(FromDto);

    /// <summary>
    /// Serializes an insert command
    /// </summary>
    /// <param name="command">Insert command to serialize</param>
    /// <returns>Serialized insert command</returns>
    public Result<string> Serialize(InsertCommand command)
        => ResultExtensions.AsResult(()
            => ToDto(command)
                .Map(commandDto => JsonSerializer.Serialize(commandDto, _serializerOptions)));

    /// <summary>
    /// Converts an insert command to its DTO
    /// </summary>
    /// <param name="insertCommand">Insert command</param>
    /// <returns>Insert command DTO</returns>
    internal Result<InsertCommandDto> ToDto(InsertCommand insertCommand)
        => ResultExtensions.AsResult(() =>
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
        => ResultExtensions.AsResult(() =>
        {
            var tabularData = _tabularDataSerializer.FromDto(insertCommandDto.Instantiation).Data!;

            var insertCommand = InsertCommandOpenBuilder.InitOpenInsert(insertCommandDto.OnTableauId)
                                    .WithInstantiation(conf => conf.WithValues(tabularData))
                                    .Build();

            return insertCommand;
        });
}
