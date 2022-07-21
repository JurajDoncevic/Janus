using Janus.Commons.CommandModels;

namespace Janus.Commons.Messages;

/// <summary>
/// Types of command requests 
/// </summary>
public enum CommandReqTypes
{
    INSERT,
    UPDATE,
    DELETE
}
public static class CommandMessageTypesExtensions
{
    public static CommandReqTypes DetermineMessageType(this BaseCommand command)
        => command.GetType() switch
        {
            Type t when t.Equals(typeof(InsertCommand)) => CommandReqTypes.INSERT,
            Type t when t.Equals(typeof(UpdateCommand)) => CommandReqTypes.UPDATE,
            Type t when t.Equals(typeof(DeleteCommand)) => CommandReqTypes.DELETE,
            _ => throw new NotImplementedException()
        };

    public static Type DetermineCommandType(this CommandReqTypes commandMessageType)
        => commandMessageType switch
        {
            CommandReqTypes.INSERT => typeof(InsertCommand),
            CommandReqTypes.UPDATE => typeof(UpdateCommand),
            CommandReqTypes.DELETE => typeof(DeleteCommand),
            _ => throw new NotImplementedException()
        };
}