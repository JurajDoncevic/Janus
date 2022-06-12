using FunctionalExtensions.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Utils.Logging;

public interface ILogger<T>
{
    Unit Debug(string message);
    Unit Debug<T1>(string messageTemplate, T1 arg1);
    Unit Debug<T1, T2>(string messageTemplate, T1 arg1, T2 arg2);
    Unit Debug<T1, T2, T3>(string messageTemplate, T1 arg1, T2 arg2, T3 arg3);
    Unit Info(string message);
    Unit Info<T1>(string messageTemplate, T1 arg1);
    Unit Info<T1, T2>(string messageTemplate, T1 arg1, T2 arg2);
    Unit Info<T1, T2, T3>(string messageTemplate, T1 arg1, T2 arg2, T3 arg3);
    Unit Warn(string message);
    Unit Warn<T1>(string messageTemplate, T1 arg1);
    Unit Warn<T1, T2>(string messageTemplate, T1 arg1, T2 arg2);
    Unit Warn<T1, T2, T3>(string messageTemplate, T1 arg1, T2 arg2, T3 arg3);
    Unit Error(string message);
    Unit Error<T1>(string messageTemplate, T1 arg1);
    Unit Error<T1, T2>(string messageTemplate, T1 arg1, T2 arg2);
    Unit Error<T1, T2, T3>(string messageTemplate, T1 arg1, T2 arg2, T3 arg3);
    Unit Critical(string message);
    Unit Critical<T1>(string messageTemplate, T1 arg1);
    Unit Critical<T1, T2>(string messageTemplate, T1 arg1, T2 arg2);
    Unit Critical<T1, T2, T3>(string messageTemplate, T1 arg1, T2 arg2, T3 arg3);
}
