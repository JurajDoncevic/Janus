namespace Janus.Commons.DataModels.Exceptions;

public class IncompatibleDotNetTypeException : Exception
{
    public IncompatibleDotNetTypeException(string columnName, Type type)
        : base($"Incompatible dotnet type {type.FullName} is used to store value for {columnName}")
    {
    }
}