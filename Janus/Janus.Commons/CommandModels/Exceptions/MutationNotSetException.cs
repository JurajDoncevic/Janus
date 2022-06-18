namespace Janus.Commons.CommandModels.Exceptions;

public class MutationNotSetException : Exception
{
    public MutationNotSetException()
        : base("Mutation clause for the command is not set.")
    {
    }
}