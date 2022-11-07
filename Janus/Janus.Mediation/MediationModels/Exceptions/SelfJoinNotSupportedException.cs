namespace Janus.Mediation.MediationModels.Exceptions;

public class SelfJoinNotSupportedException : Exception
{
    internal SelfJoinNotSupportedException(string tableauId)
        : base($"Self join on tableau {tableauId} not supported.")
    {
    }
}
