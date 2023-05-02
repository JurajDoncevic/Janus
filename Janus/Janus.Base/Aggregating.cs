using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Base
{
    public static class Aggregating
    {
        /// <summary>
        /// Used to fold a IEnumerable into a single value.
        /// E[T] -> R -> (T -> R -> R) -> R
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="ts">Target enumerable</param>
        /// <param name="initalizer">Initial value</param>
        /// <param name="foldingFunc">Accumulation function</param>
        /// <returns></returns>
        public static R Fold<T, R>(this IEnumerable<T> ts, R seed, Func<T, R, R> foldingFunc)
        {
            R result = seed;
            foreach (T item in ts)
            {
                result = foldingFunc(item, result);
            }

            return result;
        }

        /// <summary>
        /// Used to async fold a IEnumerable into a single value with element index as parameter to folding func.
        /// E[T] -> R -> (T -> R -> R) -> R
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="ts">Target enumerable</param>
        /// <param name="initalizer">Initial value</param>
        /// <param name="foldingFunc">Accumulation function</param>
        /// <returns></returns>
        public static R Foldi<T, R>(this IEnumerable<T> ts, R seed, Func<long, T, R, R> foldingFunc)
        {
            R result = seed;
            long idx = 0;
            foreach (T item in ts)
            {
                result = foldingFunc(idx, item, result);
                idx++;
            }

            return result;
        }

        /// <summary>
        /// Used to async fold a IEnumerable into a single value.
        /// E[T] -> R -> (T -> R -> R) -> R
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="ts">Target enumerable</param>
        /// <param name="initalizer">Initial value</param>
        /// <param name="foldingFunc">Accumulation function</param>
        /// <returns></returns>
        public static async Task<R> Fold<T, R>(this Task<IEnumerable<T>> ts, R seed, Func<T, R, R> foldingFunc)
        {
            R result = seed;
            foreach (T item in await ts)
            {
                result = foldingFunc(item, result);
            }

            return result;
        }

        /// <summary>
        /// Used to async fold a IEnumerable into a single value with element index as parameter to folding func.
        /// E[T] -> R -> (T -> R -> R) -> R
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="ts">Target enumerable</param>
        /// <param name="initalizer">Initial value</param>
        /// <param name="foldingFunc">Accumulation function</param>
        /// <returns></returns>
        public static async Task<R> Foldi<T, R>(this Task<IEnumerable<T>> ts, R seed, Func<long, T, R, R> foldingFunc)
        {
            R result = seed;
            long idx = 0;
            foreach (T item in await ts)
            {
                result = foldingFunc(idx, item, result);
                idx++;
            }

            return result;
        }
    }
}
