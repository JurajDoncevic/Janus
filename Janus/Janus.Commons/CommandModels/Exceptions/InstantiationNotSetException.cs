namespace Janus.Commons.CommandModels.Exceptions;

public class InstantiationNotSetException : Exception
{
    public InstantiationNotSetException()
        : base("Instantiation clause for the command is not set.")
    {
    }
}