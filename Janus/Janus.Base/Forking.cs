using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Base
{
    public static class Forking
    {
        /// <summary>
        /// Executes the prong functions and finalizes the result with the finalize function. Used to aggergate multiple function results.
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="target">Target object</param>
        /// <param name="finalizeFunc">Finalizing function</param>
        /// <param name="prongs">Prong function</param>
        /// <returns></returns>
        public static TOutput Fork<TInput, TProngOutput, TOutput>(this TInput target, Func<IEnumerable<TProngOutput>, TOutput> finalizeFunc,
                                                    params Func<TInput, TProngOutput>[] prongs)
            => prongs.AsParallel()
                     .Map(_ => _(target))
                     .Identity()
                     .Map(finalizeFunc)
                     .Data;

        /// <summary>
        /// Executes the prong functions and finalizes the result with the finalize function. Used to aggergate multiple function results.
        /// </summary>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="finalizeFunc">Finalizing function</param>
        /// <param name="prongs">Prong
        public static TOutput Fork<TProngOutput, TOutput>(Func<IEnumerable<TProngOutput>, TOutput> finalizeFunc, params Func<TProngOutput>[] prongs)
            => prongs.AsParallel()
                     .Map(_ => _.Invoke())
                     .Identity()
                     .Map(finalizeFunc)
                     .Data;


        /// <summary>
        /// Executes the prong functions and finalizes the result with the finalize function. Used to aggergate multiple function results.
        /// </summary>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="finalizeFunc">Finalizing function</param>
        /// <param name="prongs">Prong
        public static async Task<TOutput> Fork<TProngOutput, TOutput>(Func<IEnumerable<Task<TProngOutput>>, Task<TOutput>> finalizeFunc, params Func<Task<TProngOutput>>[] prongs)
            => await prongs.Map(async _ => await _())
                           .Identity()
                           .Map(finalizeFunc)
                           .Data;

        /// <summary>
        /// Executes the prong functions and finalizes the result with the finalize function. Used to aggergate multiple function results.
        /// Sync finalizing function. Use when the finalize function is small.
        /// </summary>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="finalizeFunc">Finalizing function</param>
        /// <param name="prongs">Prong
        public static async Task<TOutput> Fork<TProngOutput, TOutput>(Func<IEnumerable<TProngOutput>, TOutput> finalizeFunc, params Func<Task<TProngOutput>>[] prongs)
            => (await Task.WhenAll(prongs.Map(async _ => await _())))
                    .Identity()
                    .Map(finalizeFunc)
                    .Data;

        /// <summary>
        /// Executes the prong functions and finalizes the result with the finalize function. Used to aggergate multiple function results.
        /// Fully async
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="target">Target object</param>
        /// <param name="finalizeFunc">Finalizing function</param>
        /// <param name="prongs">Prong
        public static async Task<TOutput> Fork<TInput, TProngOutput, TOutput>(this TInput target, Func<IEnumerable<Task<TProngOutput>>, Task<TOutput>> finalizeFunc, params Func<TInput, Task<TProngOutput>>[] prongs)
            => await prongs.Map(async _ => await _(target))
                           .Identity()
                           .Map(finalizeFunc)
                           .Data;


        /// <summary>
        /// Executes the prong functions and finalizes the result with the finalize function. Used to aggergate multiple function results.
        /// Sync finalizing function. Use when the finalize function is small.
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="target">Target object</param>
        /// <param name="finalizeFunc">Finalizing function</param>
        /// <param name="prongs">Prong
        public static async Task<TOutput> Fork<TInput, TProngOutput, TOutput>(this TInput target, Func<IEnumerable<TProngOutput>, TOutput> finalizeFunc, params Func<TInput, Task<TProngOutput>>[] prongs)
            => (await Task.WhenAll(prongs.Map(async _ => await _(target))))
                    .Identity()
                    .Map(finalizeFunc)
                    .Data;
    }
}
