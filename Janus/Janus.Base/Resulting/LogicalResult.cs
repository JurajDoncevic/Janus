using System;
using System.Collections.Generic;

namespace Janus.Base.Resulting;

/// <summary>
/// Logical result of an operation
/// </summary>
public struct Result
{
    private readonly string _message;
    private readonly ResultTypes _resultType;
    private readonly Exception? _exception;

    /// <summary>
    /// Is the operation a succcess?
    /// </summary>
    public bool IsSuccess => _resultType == ResultTypes.SUCCESS;
    /// <summary>
    /// Is the operation a failure?
    /// </summary>
    public bool IsFailure => _resultType == ResultTypes.FAILURE;
    /// <summary>
    /// Did the operation throw an exception?
    /// </summary>
    public bool IsException => _resultType == ResultTypes.EXCEPTION;
    /// <summary>
    /// Has the result an exception?
    /// </summary>
    public bool HasException => _exception != null;

    /// <summary>
    /// Result type
    /// </summary>
    public ResultTypes ResultType => _resultType;
    /// <summary>
    /// Possible exception - don't use unless result type is EXCEPTION
    /// </summary>
    public Exception Exception => _exception!;
    /// <summary>
    /// Result message
    /// </summary>
    public string Message => _message;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="resultType">Result type</param>
    /// <param name="message">Result message</param>
    /// <param name="exception">Possible exception</param>
    internal Result(ResultTypes resultType, string? message = null, Exception? exception = null)
    {
        _exception = exception;
        // if no exception is given then, it's a failure - just in case
        _resultType = resultType == ResultTypes.EXCEPTION && _exception == null ? ResultTypes.FAILURE : resultType;

        // if no message is given, set the message according to the result type
        _message = message ?? resultType switch
        {
            ResultTypes.SUCCESS => "Operation successful",
            ResultTypes.FAILURE => "Operation failed",
            ResultTypes.EXCEPTION => $"Operation failed with exception: {exception!.Message}", // can't be null at this point; takes exception message
            _ => "UNKNOWN" // never gets to this, just to stop the warn
        };
    }

    /// <summary>
    /// Implicit bool operator
    /// </summary>
    /// <param name="result"></param>
    public static implicit operator bool(Result result)
        => result.IsSuccess;

    /// <summary>
    /// Implicit operator to turn a bool into a Result
    /// </summary>
    /// <param name="isSuccess"></param>
    public static implicit operator Result(bool isSuccess)
        => isSuccess
        ? Results.OnSuccess()
        : Results.OnFailure();

    public override bool Equals(object? obj)
    {
        return obj is Result result &&
               _message == result._message &&
               _resultType == result._resultType &&
               EqualityComparer<Exception?>.Default.Equals(_exception, result._exception);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_message, _resultType, _exception);
    }
}

public static partial class Results
{
    /// <summary>
    /// Creates a successful Result
    /// </summary>
    /// <param name="message">Outcome message</param>
    /// <returns></returns>
    public static Result OnSuccess(string message = "Operation successful")
         => new Result(ResultTypes.SUCCESS, message);

    /// <summary>
    /// Creates a failure Result
    /// </summary>
    /// <param name="message">Outcome message</param>
    /// <returns></returns>
    public static Result OnFailure(string message = "Operation failed")
        => new Result(ResultTypes.FAILURE, message);

    /// <summary>
    /// Creates an exception Result
    /// </summary>
    /// <param name="exception">Thrown exception</param>
    /// <returns></returns>
    public static Result OnException(Exception exception, string? message = null)
    {
        exception ??= new Exception("Unknown exception"); // don't throw exception on null, keep it a monad
        return new Result(ResultTypes.EXCEPTION, message, exception);
    }
}