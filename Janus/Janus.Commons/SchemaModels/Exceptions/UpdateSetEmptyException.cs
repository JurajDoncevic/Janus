namespace Janus.Commons.SchemaModels.Exceptions;
public class UpdateSetEmptyException : Exception
{
    internal UpdateSetEmptyException()
    : base($"Update set can't be empty")
    {

    }
}
