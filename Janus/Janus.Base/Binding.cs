using Janus.Base.Resulting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Base
{
    /// <summary>
    /// Binding extensions for system types
    /// </summary>
    public static class Binding
    {
        /// <summary>
        /// Bind over IEnumerable (SelectMany): E[T]->(T->E[R])->E[R]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="target"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IEnumerable<R> Bind<T, R>(this IEnumerable<T> target, Func<T, IEnumerable<R>> func)
        {
            foreach (T t in target)
                foreach (R r in func(t))
                    yield return r;
        }

        /// <summary>
        /// Bind over Task. Chains Task awaiting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="target"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static async Task<R> BindTask<T, R>(this Task<T> target, Func<T, Task<R>> func)
        {
            return await func(await target);
        }
    }
}
