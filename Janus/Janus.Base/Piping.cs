using Janus.Base;
using System;
using System.Collections.Generic;
using System.Text;

#nullable enable
namespace Janus.Base
{
    public static class Piping
    {

        /// <summary>
        /// Executes an Action returning/side-effect function with target as the parameter and passes the target onward in the pipeline.
        /// Side effects preserved if done on a mutable-type target (class)! 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">Target object</param>
        /// <param name="action">Side-effect action</param>
        /// <returns></returns>
        public static T Pass<T>(this T target, Action<T> action)
        {
            action(target);
            return target;
        }

        /// <summary>
        /// Executes an Action returning/side-effect function with target as the parameter and passes the target onward in the pipeline.
        /// Side effects preserved if done on a mutable-type target (class)! 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">Target object</param>
        /// <param name="successAction">Side-effect action on success</param>
        /// <param name="failureAction">Side-effect action on failure</param>
        /// <remarks>This should be used for logging</remarks>
        /// <returns></returns>
        public static Resulting.Result<T> Pass<T>(this Resulting.Result<T> target, 
                                                    Action<Resulting.Result<T>> successAction,
                                                    Action<Resulting.Result<T>> failureAction)
        {
            if (target.IsSuccess)
            {
                successAction(target);
            }
            else
            {
                failureAction(target);
            }
                
            return target;
        }

        /// <summary>
        /// Executes a Unit returning/side-effect function with target as the parameter and passes the target onward in the pipeline.
        /// Side effects preserved if done on a mutable-type target (class)! 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">Target object</param>
        /// <param name="successFunc">Side-effect action on success</param>
        /// <param name="failureFunc">Side-effect action on failure</param>
        /// <remarks>This should be used for logging</remarks>
        /// <returns></returns>
        public static Resulting.Result<T> Pass<T>(this Resulting.Result<T> target,
                                                    Func<Resulting.Result<T>, Unit> successFunc,
                                                    Func<Resulting.Result<T>, Unit> failureFunc)
        {
            if (target.IsSuccess)
            {
                successFunc(target);
            }
            else
            {
                failureFunc(target);
            }

            return target;
        }

        /// <summary>
        /// Executes an Action returning/side-effect function with target as the parameter and passes the target onward in the pipeline.
        /// Side effects preserved if done on a mutable-type target (class)! 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">Target object</param>
        /// <param name="successAction">Side-effect action on success</param>
        /// <param name="failureAction">Side-effect action on failure</param>
        /// <returns></returns>
        public static Resulting.Result Pass(this Resulting.Result target,
                                          Action<Resulting.Result> successAction,
                                          Action<Resulting.Result> failureAction)
        {
            if (target.IsSuccess)
            {
                successAction(target);
            }
            else
            {
                failureAction(target);
            }

            return target;
        }

        /// <summary>
        /// Executes a Unit returning/side-effect function with target as the parameter and passes the target onward in the pipeline.
        /// Side effects preserved if done on a mutable-type target (class)! 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">Target object</param>
        /// <param name="successFunc">Side-effect function on success</param>
        /// <param name="failureFunc">Side-effect function on failure</param>
        /// <returns></returns>
        public static Resulting.Result Pass(this Resulting.Result target,
                                          Func<Resulting.Result, Unit> successFunc,
                                          Func<Resulting.Result, Unit> failureFunc)
        {
            if (target.IsSuccess)
            {
                successFunc(target);
            }
            else
            {
                failureFunc(target);
            }

            return target;
        }

        /// <summary>
        /// Executes a Unit returning/side-effect function with target as the parameter and passes the target onward in the pipeline.
        /// Side effects preserved if done on a mutable-type target (class)! 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">Target object</param>
        /// <param name="func">Side-effect function</param>
        /// <returns></returns>
        public static T Pass<T>(this T target, Func<T, Unit> func)
        {
            func(target);
            return target;
        }


        /// <summary>
        /// Passes argument to function if it is not equal to null. Turns classes into Monads.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="target">Target object</param>
        /// <param name="func">Operation function</param>
        /// <returns></returns>
        public static R? IfNotNull<T, R>(this T? target, Func<T, R> func)
            where T : class
            where R : class
            => target != null
                ? (R?)func(target)
                : null;

        /// <summary>
        /// Passes argument to function if it is not equal to null. Turns structs into Monads.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="target">Target object</param>
        /// <param name="func">Operation function</param>
        /// <returns></returns>
        public static R? IfNotNull<T, R>(this T? target, Func<T, R?> func)
            where T : struct
            where R : struct
            => target.HasValue
                ? (R?)func(target.Value)
                : null;

        /// <summary>
        /// Passes argument to function if it is not equal to null. Natural transformation between classes and structs.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="target">Target object</param>
        /// <param name="func">Operation function</param>
        /// <returns></returns>
        public static R? IfNotNull<T, R>(this T? target, Func<T, R?> func)
            where T : class
            where R : struct
            => target != null
                ? (R?)func(target)
                : null;

        /// <summary>
        /// Passes argument to function if it is not equal to null. Natural transformation between classes and structs.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="target">Target object</param>
        /// <param name="func">Operation function</param>
        /// <returns></returns>
        public static R? IfNotNull<T, R>(this T? target, Func<T, R?> func)
            where T : struct
            where R : class
            => target.HasValue
                ? (R?)func(target.Value)
                : null;

    }
}

