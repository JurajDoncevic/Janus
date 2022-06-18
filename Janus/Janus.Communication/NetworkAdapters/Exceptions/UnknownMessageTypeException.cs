namespace Janus.Communication.NetworkAdapters.Exceptions;

public class UnknownMessageTypeException : Exception
{
    public UnknownMessageTypeException(string? preamble)
        : base($"Unknown message type received {preamble}")
    {
    }
}
