namespace Janus.Mediation.MediationModels.Exceptions;

public class JoinsNotConnectedException : Exception
{
    public JoinsNotConnectedException()
        : base("Given joins don't create an unified tableau - joins don't create a connected join graph.")
    {
    }
}
