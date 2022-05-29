using System.Runtime.Serialization;

namespace Janus.Commons.DataModels.Exceptions;

public class IncompatibleDotNetTypeException : Exception
{
    public IncompatibleDotNetTypeException(string attrId, Type type)
        : base($"Incompatible dotnet type {type.FullName} is used to store value for {attrId}")
    {
    }
}