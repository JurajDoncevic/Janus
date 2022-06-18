namespace Janus.Commons.CommandModels.Exceptions;

public class MutationOnPrimaryKeyNotAllowedException : Exception
{
    public MutationOnPrimaryKeyNotAllowedException(string attributeName)
        : base($"Can't mutate value on primary key attribute {attributeName}")
    {
    }
}
