using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Base
{
    public static class Validation
    {
        /// <summary>
        /// Checks if the given input object is valid by all the given predicates
        /// </summary>
        /// <typeparam name="TInput">Type of the input object</typeparam>
        /// <param name="target">Target object that is to be validated</param>
        /// <param name="predicates">Predicates to check validity</param>
        /// <returns>True if passes predicates, else false</returns>
        public static bool Validate<TInput>(this TInput target, params Func<TInput, bool>[] predicates) 
            => predicates.AsParallel().All(_ => _(target));


        /// <summary>
        /// Checks if the given input object is valid by all the given predicates
        /// </summary>
        /// <typeparam name="TInput">Type of the input object</typeparam>
        /// <param name="target">Target object that is to be validated</param>
        /// <param name="predicates">Predicates to check validity</param>
        /// <returns>True if passes predicates, else false</returns>
        public static async Task<bool> Validate<TInput>(this TInput target, params Func<TInput, Task<bool>>[] predicates) 
            => (await Task.WhenAll(predicates.Map(async _ => await _(target))))
                    .All(_ => _);


        /// <summary>
        /// Creates a validator function
        /// </summary>
        /// <typeparam name="TInput">Type of the input object</typeparam>
        /// <param name="target">Target object that is to be validated</param>
        /// <param name="predicates">Predicates to check validity</param>
        /// <returns>True if passes predicates, else false</returns>
        public static Func<TInput, bool> Validator<TInput>(params Func<TInput, bool>[] predicates) 
            => (TInput target) 
                => predicates.AsParallel().All(_ => _(target));

        /// <summary>
        /// Creates a validator function
        /// </summary>
        /// <typeparam name="TInput">Type of the input object</typeparam>
        /// <param name="target">Target object that is to be validated</param>
        /// <param name="predicates">Predicates to check validity</param>
        /// <returns>True if passes predicates, else false</returns>
        public static Func<TInput, Task<bool>> Validator<TInput>(params Func<TInput, Task<bool>>[] predicates) 
            => async (TInput target) 
                => (await Task.WhenAll(predicates.Map(async _ => await _(target))))
                    .All(_ => _);
    }
}
