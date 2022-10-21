namespace Janus.Commons.CommandModels.Exceptions;
public class CommandAllowedOnTableauWideUpdateSetException : Exception
{
    public CommandAllowedOnTableauWideUpdateSetException()
        : base("This command can only be constructed on a tableau with a tableau-wide update set")
    {
    }
}
