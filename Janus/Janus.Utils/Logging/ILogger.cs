using FunctionalExtensions.Base;

namespace Janus.Utils.Logging;

/// <summary>
/// Generic interface for library logging
/// </summary>
public interface ILogger
{
    public LoggerAdapter<T> ResolveLogger<T>();
}

/// <summary>
/// Specialized interface for library logging
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ILogger<T>
{
    /// <summary>
    /// Is the Debug level enabled?
    /// </summary>
    Func<bool> IsDebugEnabled { get; }
    /// <summary>
    /// Is the Info level enabled?
    /// </summary>
    Func<bool> IsInfoEnabled { get; }
    /// <summary>
    /// Is the Warn level enabled?
    /// </summary>
    Func<bool> IsWarnEnabled { get; }
    /// <summary>
    /// Is the Error level enabled?
    /// </summary>
    Func<bool> IsErrorEnabled { get; }
    /// <summary>
    /// Is the Critical level enabled?
    /// </summary>
    Func<bool> IsCriticalEnabled { get; }

    /// <summary>
    /// Logs a debug level message
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Unit Debug(string message);
    /// <summary>
    /// Logs a debug level message with arguments
    /// </summary>
    /// <param name="messageTemplate"></param>
    /// <returns></returns>
    Unit Debug<T1>(string messageTemplate, T1 arg1);
    /// <summary>
    /// Logs a debug level message with arguments
    /// </summary>
    /// <param name="messageTemplate"></param>
    /// <returns></returns>
    Unit Debug<T1, T2>(string messageTemplate, T1 arg1, T2 arg2);
    /// <summary>
    /// Logs a debug level message with arguments
    /// </summary>
    /// <param name="messageTemplate"></param>
    /// <returns></returns>
    Unit Debug<T1, T2, T3>(string messageTemplate, T1 arg1, T2 arg2, T3 arg3);
    /// <summary>
    /// Logs a debug level message with arguments
    /// </summary>
    /// <param name="messageTemplate"></param>
    /// <returns></returns>
    Unit Debug<T1, T2, T3, T4>(string messageTemplate, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    /// <summary>
    /// Logs an info level message
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Unit Info(string message);
    /// <summary>
    /// Logs an info level message with arguments
    /// </summary>
    /// <param name="messageTemplate"></param>
    /// <returns></returns>
    Unit Info<T1>(string messageTemplate, T1 arg1);
    /// <summary>
    /// Logs an info level message with arguments
    /// </summary>
    /// <param name="messageTemplate"></param>
    /// <returns></returns>
    Unit Info<T1, T2>(string messageTemplate, T1 arg1, T2 arg2);
    /// <summary>
    /// Logs an info level message with arguments
    /// </summary>
    /// <param name="messageTemplate"></param>
    /// <returns></returns>
    Unit Info<T1, T2, T3>(string messageTemplate, T1 arg1, T2 arg2, T3 arg3);
    /// <summary>
    /// Logs an info level message with arguments
    /// </summary>
    /// <param name="messageTemplate"></param>
    /// <returns></returns>
    Unit Info<T1, T2, T3, T4>(string messageTemplate, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    /// <summary>
    /// Logs a warn level message
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Unit Warn(string message);
    /// <summary>
    /// Logs a warn level message with arguments
    /// </summary>
    /// <param name="messageTemplate"></param>
    /// <returns></returns>
    Unit Warn<T1>(string messageTemplate, T1 arg1);
    /// <summary>
    /// Logs a warn level message with arguments
    /// </summary>
    /// <param name="messageTemplate"></param>
    /// <returns></returns>
    Unit Warn<T1, T2>(string messageTemplate, T1 arg1, T2 arg2);
    /// <summary>
    /// Logs a warn level message with arguments
    /// </summary>
    /// <param name="messageTemplate"></param>
    /// <returns></returns>
    Unit Warn<T1, T2, T3>(string messageTemplate, T1 arg1, T2 arg2, T3 arg3);
    /// <summary>
    /// Logs a warn level message with arguments
    /// </summary>
    /// <param name="messageTemplate"></param>
    /// <returns></returns>
    Unit Warn<T1, T2, T3, T4>(string messageTemplate, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    /// <summary>
    /// Logs an error level message
    /// </summary>
    /// <param name="message"></param>
    /// <remarks></remarks>
    /// <returns></returns>
    Unit Error(string message);
    /// <summary>
    /// Logs an error level message with arguments
    /// </summary>
    /// <param name="messageTemplate"></param>
    /// <returns></returns>
    Unit Error<T1>(string messageTemplate, T1 arg1);
    /// <summary>
    /// Logs an error level message with arguments
    /// </summary>
    /// <param name="messageTemplate"></param>
    /// <returns></returns>
    Unit Error<T1, T2>(string messageTemplate, T1 arg1, T2 arg2);
    /// <summary>
    /// Logs an error level message with arguments
    /// </summary>
    /// <param name="messageTemplate"></param>
    /// <returns></returns>
    Unit Error<T1, T2, T3>(string messageTemplate, T1 arg1, T2 arg2, T3 arg3);
    /// <summary>
    /// Logs an error level message with arguments
    /// </summary>
    /// <param name="messageTemplate"></param>
    /// <returns></returns>
    Unit Error<T1, T2, T3, T4>(string messageTemplate, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    /// <summary>
    /// Logs a critical level message
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Unit Critical(string message);
    /// <summary>
    /// Logs a critical level message with arguments
    /// </summary>
    /// <param name="messageTemplate"></param>
    /// <returns></returns>
    Unit Critical<T1>(string messageTemplate, T1 arg1);
    /// <summary>
    /// Logs a critical level message with arguments
    /// </summary>
    /// <param name="messageTemplate"></param>
    /// <returns></returns>
    Unit Critical<T1, T2>(string messageTemplate, T1 arg1, T2 arg2);
    /// <summary>
    /// Logs a critical level message with arguments
    /// </summary>
    /// <param name="messageTemplate"></param>
    /// <returns></returns>
    Unit Critical<T1, T2, T3>(string messageTemplate, T1 arg1, T2 arg2, T3 arg3);
    /// <summary>
    /// Logs a critical level message with arguments
    /// </summary>
    /// <param name="messageTemplate"></param>
    /// <returns></returns>
    Unit Critical<T1, T2, T3, T4>(string messageTemplate, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
}