using Janus.Base.Resulting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Base
{
    public static class Awaiting
    {
        /// <summary>
        /// Awaits a task result and operates over it. Blocks thread!
        /// Uses Task.Result call - not to be used lightly!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="task"></param>
        /// <param name="operation">Operation to execute over result</param>
        /// <returns>Sync result of operation</returns>
        public static R WaitFor<T, R>(this Task<T> task, Func<T, R> operation)
            => operation(task.Result);

        /// <summary>
        /// Awaits a task result until timeout and operates over it if finished. Blocks thread!
        /// Uses Task.Result call - not to be used lightly!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="task"></param>
        /// <param name="operation">Operation to execute over result</param>
        /// <param name="timeoutMillis"></param>
        /// <returns>Sync result of operation or default value of return type</returns>
        public static R WaitFor<T, R>(this Task<T> task, Func<T, R> operation, int timeoutMillis)
            => task.Wait(timeoutMillis)
                ? operation(task.Result)
                : default(R);

    }
}
