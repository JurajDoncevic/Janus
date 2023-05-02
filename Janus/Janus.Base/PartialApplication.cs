using System;
using System.Collections.Generic;
using System.Text;

namespace Janus.Base
{
    public static class PartialApplication
    {
        /// <summary>
        /// Applies the first parameter to function and returns a new function. Given parameter is put in a CLOSURE
        /// </summary>
        /// <param name="func">Function to apply to</param>
        /// <param name="param">Parameter to apply</param>
        /// <returns>Function with 1st param applied</returns>
        public static Func<R> Apply<T1, R>(this Func<T1, R> func, T1 param) =>
            () => func(param);

        /// <summary>
        /// Applies the first parameter to function and returns a new function. Given parameter is put in a CLOSURE
        /// </summary>
        /// <param name="func">Function to apply to</param>
        /// <param name="param">Parameter to apply</param>
        /// <returns>Function with 1st param applied</returns>
        public static Func<T2, R> Apply<T1, T2, R>(this Func<T1, T2, R> func, T1 param) =>
            (t2) => func(param, t2);

        /// <summary>
        /// Applies the first parameter to function and returns a new function. Given parameter is put in a CLOSURE
        /// </summary>
        /// <param name="func">Function to apply to</param>
        /// <param name="param">Parameter to apply</param>
        /// <returns>Function with 1st param applied</returns>
        public static Func<T2, T3, R> Apply<T1, T2, T3, R>(this Func<T1, T2, T3, R> func, T1 param) =>
            (t2, t3) => func(param, t2, t3);

        /// <summary>
        /// Applies the first parameter to function and returns a new function. Given parameter is put in a CLOSURE
        /// </summary>
        /// <param name="func">Function to apply to</param>
        /// <param name="param">Parameter to apply</param>
        /// <returns>Function with 1st param applied</returns>
        public static Func<T2, T3, T4, R> Apply<T1, T2, T3, T4, R>(this Func<T1, T2, T3, T4, R> func, T1 param) =>
            (t2, t3, t4) => func(param, t2, t3, t4);

        /// <summary>
        /// Applies the first parameter to function and returns a new function. Given parameter is put in a CLOSURE
        /// </summary>
        /// <param name="func">Function to apply to</param>
        /// <param name="param">Parameter to apply</param>
        /// <returns>Function with 1st param applied</returns>
        public static Func<T2, T3, T4, T5, R> Apply<T1, T2, T3, T4, T5, R>(this Func<T1, T2, T3, T4, T5, R> func, T1 param) =>
            (t2, t3, t4, t5) => func(param, t2, t3, t4, t5);

        /// <summary>
        /// Applies the first parameter to function and returns a new function. Given parameter is put in a CLOSURE
        /// </summary>
        /// <param name="func">Function to apply to</param>
        /// <param name="param">Parameter to apply</param>
        /// <returns>Function with 1st param applied</returns>
        public static Func<T2, T3, T4, T5, T6, R> Apply<T1, T2, T3, T4, T5, T6, R>(this Func<T1, T2, T3, T4, T5, T6, R> func, T1 param) =>
            (t2, t3, t4, t5, t6) => func(param, t2, t3, t4, t5, t6);

        /// <summary>
        /// Applies the first parameter to function and returns a new function. Given parameter is put in a CLOSURE
        /// </summary>
        /// <param name="func">Function to apply to</param>
        /// <param name="param">Parameter to apply</param>
        /// <returns>Function with 1st param applied</returns>
        public static Func<T2, T3, T4, T5, T6, T7, R> Apply<T1, T2, T3, T4, T5, T6, T7, R>(this Func<T1, T2, T3, T4, T5, T6, T7, R> func, T1 param) =>
            (t2, t3, t4, t5, t6, t7) => func(param, t2, t3, t4, t5, t6, t7);

        /// <summary>
        /// Applies the first parameter to function and returns a new function. Given parameter is put in a CLOSURE
        /// </summary>
        /// <param name="func">Function to apply to</param>
        /// <param name="param">Parameter to apply</param>
        /// <returns>Function with 1st param applied</returns>
        public static Func<T2, T3, T4, T5, T6, T7, T8, R> Apply<T1, T2, T3, T4, T5, T6, T7, T8, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, R> func, T1 param) =>
            (t2, t3, t4, t5, t6, t7, t8) => func(param, t2, t3, t4, t5, t6, t7, t8);

        /// <summary>
        /// Applies the first parameter to function and returns a new function. Given parameter is put in a CLOSURE
        /// </summary>
        /// <param name="func">Function to apply to</param>
        /// <param name="param">Parameter to apply</param>
        /// <returns>Function with 1st param applied</returns>
        public static Func<T2, T3, T4, T5, T6, T7, T8, T9, R> Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> func, T1 param) =>
            (t2, t3, t4, t5, t6, t7, t8, t9) => func(param, t2, t3, t4, t5, t6, t7, t8, t9);

        /// <summary>
        /// Applies the first parameter to function and returns a new function. Given parameter is put in a CLOSURE
        /// </summary>
        /// <param name="func">Function to apply to</param>
        /// <param name="param">Parameter to apply</param>
        /// <returns>Function with 1st param applied</returns>
        public static Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, R> Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R> func, T1 param) =>
            (t2, t3, t4, t5, t6, t7, t8, t9, t10) => func(param, t2, t3, t4, t5, t6, t7, t8, t9, t10);

        /// <summary>
        /// Applies the first parameter to function and returns a new function. Given parameter is put in a CLOSURE
        /// </summary>
        /// <param name="func">Function to apply to</param>
        /// <param name="param">Parameter to apply</param>
        /// <returns>Function with 1st param applied</returns>
        public static Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R> Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R> func, T1 param) =>
            (t2, t3, t4, t5, t6, t7, t8, t9, t10, t11) => func(param, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);

        /// <summary>
        /// Applies the first parameter to function and returns a new function. Given parameter is put in a CLOSURE
        /// </summary>
        /// <param name="func">Function to apply to</param>
        /// <param name="param">Parameter to apply</param>
        /// <returns>Function with 1st param applied</returns>
        public static Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R> Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R> func, T1 param) =>
            (t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12) => func(param, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
    }
}
