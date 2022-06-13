﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FunctionalExtensions.Base;
using static FunctionalExtensions.Base.UnitExtensions;
using Microsoft.Extensions.Logging;

namespace Janus.Utils.Logging;

public class LoggerAdapter<T> : ILogger<T>
{
    private readonly Microsoft.Extensions.Logging.ILogger<T> _logger;

    public LoggerAdapter(Microsoft.Extensions.Logging.ILogger<T> logger)
    {
        _logger = logger;
    }

    public Func<bool> IsDebugEnabled => () => _logger.IsEnabled(LogLevel.Debug);

    public Func<bool> IsInfoEnabled => () => _logger.IsEnabled(LogLevel.Information);

    public Func<bool> IsWarnEnabled => () => _logger.IsEnabled(LogLevel.Warning);

    public Func<bool> IsErrorEnabled => () => _logger.IsEnabled(LogLevel.Error);

    public Func<bool> IsCriticalEnabled => () => _logger.IsEnabled(LogLevel.Critical);

    public Unit Critical(string message)
    {
        if (IsCriticalEnabled())
        {
            _logger.LogCritical(message);
        }
        return Unit();
    }

    public Unit Critical<T1>(string messageTemplate, T1 arg1)
    {
        if (IsCriticalEnabled())
        {
            _logger.LogCritical(messageTemplate, arg1);
        }
        return Unit();
    }

    public Unit Critical<T1, T2>(string messageTemplate, T1 arg1, T2 arg2)
    {
        if (IsCriticalEnabled())
        {
            _logger.LogCritical(messageTemplate, arg1, arg2);
        }
        return Unit();
    }

    public Unit Critical<T1, T2, T3>(string messageTemplate, T1 arg1, T2 arg2, T3 arg3)
    {
        if (IsCriticalEnabled())
        {
            _logger.LogCritical(messageTemplate, arg1, arg2, arg3);
        }
        return Unit();
    }

    public Unit Critical<T1, T2, T3, T4>(string messageTemplate, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        if (IsCriticalEnabled())
        {
            _logger.LogCritical(messageTemplate, arg1, arg2, arg3, arg4);
        }
        return Unit();
    }

    public Unit Debug(string message)
    {
        if (IsDebugEnabled())
        {
            _logger.LogDebug(message);
        }
        return Unit();
    }

    public Unit Debug<T1>(string messageTemplate, T1 arg1)
    {
        if (IsDebugEnabled())
        {
            _logger.LogDebug(messageTemplate, arg1);
        }
        return Unit();
    }

    public Unit Debug<T1, T2>(string messageTemplate, T1 arg1, T2 arg2)
    {
        if (IsDebugEnabled())
        {
            _logger.LogDebug(messageTemplate, arg1, arg2);
        }
        return Unit();
    }

    public Unit Debug<T1, T2, T3>(string messageTemplate, T1 arg1, T2 arg2, T3 arg3)
    {
        if (IsDebugEnabled())
        {
            _logger.LogDebug(messageTemplate, arg1, arg2, arg3);
        }
        return Unit();
    }

    public Unit Debug<T1, T2, T3, T4>(string messageTemplate, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        if (IsDebugEnabled())
        {
            _logger.LogDebug(messageTemplate, arg1, arg2, arg3, arg4);
        }
        return Unit();
    }

    public Unit Error(string message)
    {
        if (IsErrorEnabled())
        {
            _logger.LogError(message);
        }
        return Unit();
    }

    public Unit Error<T1>(string messageTemplate, T1 arg1)
    {
        if (IsErrorEnabled())
        {
            _logger.LogError(messageTemplate, arg1);
        }
        return Unit();
    }

    public Unit Error<T1, T2>(string messageTemplate, T1 arg1, T2 arg2)
    {
        if (IsErrorEnabled())
        {
            _logger.LogError(messageTemplate, arg1, arg2);
        }
        return Unit();
    }

    public Unit Error<T1, T2, T3>(string messageTemplate, T1 arg1, T2 arg2, T3 arg3)
    {
        if (IsErrorEnabled())
        {
            _logger.LogError(messageTemplate, arg1, arg2, arg3);
        }
        return Unit();
    }

    public Unit Error<T1, T2, T3, T4>(string messageTemplate, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        if (IsErrorEnabled())
        {
            _logger.LogError(messageTemplate, arg1, arg2, arg3, arg4);
        }
        return Unit();
    }

    public Unit Info(string message)
    {
        if (IsInfoEnabled())
        {
            _logger.LogInformation(message);
        }
        return Unit();
    }

    public Unit Info<T1>(string messageTemplate, T1 arg1)
    {
        if (IsInfoEnabled())
        {
            _logger.LogInformation(messageTemplate, arg1);
        }
        return Unit();
    }

    public Unit Info<T1, T2>(string messageTemplate, T1 arg1, T2 arg2)
    {
        if (IsInfoEnabled())
        {
            _logger.LogInformation(messageTemplate, arg1, arg2);
        }
        return Unit();
    }

    public Unit Info<T1, T2, T3>(string messageTemplate, T1 arg1, T2 arg2, T3 arg3)
    {
        if (IsInfoEnabled())
        {
            _logger.LogInformation(messageTemplate, arg1, arg2, arg3);
        }
        return Unit();
    }

    public Unit Info<T1, T2, T3, T4>(string messageTemplate, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        if (IsInfoEnabled())
        {
            _logger.LogInformation(messageTemplate, arg1, arg2, arg3, arg4);
        }
        return Unit();
    }

    public Unit Warn(string message)
    {
        if (IsWarnEnabled())
        {
            _logger.LogWarning(message);
        }
        return Unit();
    }

    public Unit Warn<T1>(string messageTemplate, T1 arg1)
    {
        if (IsWarnEnabled())
        {
            _logger.LogWarning(messageTemplate, arg1);
        }
        return Unit();
    }

    public Unit Warn<T1, T2>(string messageTemplate, T1 arg1, T2 arg2)
    {
        if (IsWarnEnabled())
        {
            _logger.LogWarning(messageTemplate, arg1, arg2);
        }
        return Unit();
    }

    public Unit Warn<T1, T2, T3>(string messageTemplate, T1 arg1, T2 arg2, T3 arg3)
    {
        if (IsWarnEnabled())
        {
            _logger.LogWarning(messageTemplate, arg1, arg2, arg3);
        }
        return Unit();
    }

    public Unit Warn<T1, T2, T3, T4>(string messageTemplate, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        if (IsWarnEnabled())
        {
            _logger.LogWarning(messageTemplate, arg1, arg2, arg3, arg4);
        }
        return Unit();
    }
}
