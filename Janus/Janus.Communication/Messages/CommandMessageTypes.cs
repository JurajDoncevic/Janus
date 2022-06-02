using Janus.Commons.CommandModels;

namespace Janus.Communication.Messages
{
    public static class CommandMessageTypes
    {
        public static string INSERT => "INSERT";
        public static string UPDATE => "UPDATE";
        public static string DELETE => "DELETE";

        public static string DetermineMessageType(BaseCommand command)
            => command.GetType() switch
            {
                typeof(InsertCommand) => INSERT,
                typeof(UpdateCommand) => UPDATE,
                _ => throw new NotImplementedException()
            };

    }
}