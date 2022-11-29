using Janus.Commons.SchemaModels;

namespace Janus.Mediation.SchemaMediationModels.Exceptions;

public class SelfJoinNotSupportedException : Exception
{
    internal SelfJoinNotSupportedException(TableauId tableauId)
        : base($"Self join on tableau {tableauId} not supported.")
    {
    }
}
