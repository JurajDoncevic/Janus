using Janus.Commons.CommandModels;
using System.Text.Json.Serialization;

namespace Janus.Communication.Messages;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CommandMessageTypes
{
    INSERT,
    UPDATE,
    DELETE
}
public static class CommandMessageTypesExtensions
{
    public static CommandMessageTypes DetermineMessageType(this BaseCommand command)
        => command.GetType() switch
        {
            Type t when t.Equals(typeof(InsertCommand)) => CommandMessageTypes.INSERT,
            Type t when t.Equals(typeof(UpdateCommand)) => CommandMessageTypes.UPDATE,
            Type t when t.Equals(typeof(DeleteCommand)) => CommandMessageTypes.DELETE,
            _ => throw new NotImplementedException()
        };
}