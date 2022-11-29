using Janus.Commons.SchemaModels;

namespace Janus.Commons.QueryModels.Exceptions;

public class SelfJoinNotSupportedException : Exception
{
    internal SelfJoinNotSupportedException(TableauId tableauId)
        : base($"Self join on tableau {tableauId} not supported.")
    {
    }
}
