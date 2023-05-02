namespace Janus.Base.Resulting;


/// <summary>
/// Data result of an operation
/// </summary>
/// <typeparam name="TData"></typeparam>
public struct Result<TData>
{
    private readonly TData? _data;
    private readonly string _message;
    private readonly ResultTypes _resultType;
    private readonly Exception? _exception;

    /// <summary>
    /// Is the operation a succcess?
    /// </summary>
    public bool IsSuccess => _data != null && _resultType == ResultTypes.SUCCESS;
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
    /// Has the result any data?
    /// </summary>
    public bool HasData => _data != null;
    /// <summary>
    /// Result type
    /// </summary>
    public ResultTypes ResultType => _resultType;
    /// <summary>
    /// Result data - don't use unless result type is SUCCESS
    /// </summary>
    public TData Data => _data!;
    /// <summary>
    /// Thrown exception - don't use unless result type is EXCEPTION
    /// </summary>
    public Exception Exception => _exception!;
    /// <summary>
    /// Result message
    /// </summary>
    public string Message => _message;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="data">Possible data</param>
    /// <param name="resultType">Result type</param>
    /// <param name="message">Result message</param>
    /// <param name="exception">Possible exception</param>
    internal Result(TData? data, ResultTypes resultType, string? message = null, Exception? exception = null)
    {
        _data = data;
        _exception = exception;

        // check the result type against the data just in case
        _resultType = resultType == ResultTypes.SUCCESS && _data == null ? ResultTypes.FAILURE : resultType;
        // if no exception is given then, it's a failure - just in case
        _resultType = resultType == ResultTypes.EXCEPTION && _exception == null ? ResultTypes.FAILURE : _resultType;

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
    public static implicit operator bool(Result<TData> result)
        => result.IsSuccess;

    /// <summary>
    /// Implicit operator to turn a value into a Result
    /// </summary>
    /// <param name="data"></param>
    public static implicit operator Result<TData>(TData data)
        => data != null
        ? Results.OnSuccess(data)
        : Results.OnFailure<TData>();

    public override bool Equals(object? obj)
    {
        return obj is Result<TData> result &&
               EqualityComparer<TData?>.Default.Equals(_data, result._data) &&
               _message == result._message &&
               _resultType == result._resultType &&
               EqualityComparer<Exception?>.Default.Equals(_exception, result._exception);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_data, _message, _resultType, _exception);
    }
}


public static partial class Results
{
    /// <summary>
    /// Creates a successful operation result
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <param name="data">Result data</param>
    /// <param name="message">Outcome message</param>
    /// <returns></returns>
    public static Result<TData> OnSuccess<TData>(TData? data, string message = "Operation successful")
        => new Result<TData>(data, ResultTypes.SUCCESS, message, null);

    /// <summary>
    /// Creates a failure operation result
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <param name="message">Outcome message</param>
    /// <returns></returns>
    public static Result<TData> OnFailure<TData>(string? message = null)
        => new Result<TData>(default, ResultTypes.FAILURE, message); // default give null for ref types

    /// <summary>
    /// Creates an exception operation result
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <param name="exception">The thrown exception</param>
    /// <param name="message">Outcome message</param>
    /// <returns></returns>
    public static Result<TData> OnException<TData>(Exception exception, string? message = null)
    {
        exception ??= new Exception("Unknown exception");
        return new Result<TData>(default, ResultTypes.EXCEPTION, message, exception);
    }
}