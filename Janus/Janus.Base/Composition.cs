using System;
using System.Collections.Generic;
using System.Text;

namespace Janus.Base
{
    public static class Composition
    {
        /// <summary>
        /// Inverse function composition. 
        /// </summary>
        /// <param name="before">This is done first</param>
        /// <param name="after">This is done after the first</param>
        /// <returns></returns>
        public static Func<T1, R> Before<T1, T2, R>(this Func<T1, T2> before, Func<T2, R> after)
            => _ => after(before(_));

        /// <summary>
        /// Inverse function composition. 
        /// </summary>
        /// <param name="before">This is done first</param>
        /// <param name="after">This is done after the first</param>
        /// <returns></returns>
        public static Func<T1, T2, R> Before<T1, T2, T3, R>(this Func<T1, T2, T3> before, Func<T3, R> after)
            => (_1, _2) => after(before(_1, _2));

        /// <summary>
        /// Inverse function composition. 
        /// </summary>
        /// <param name="before">This is done first</param>
        /// <param name="after">This is done after the first</param>
        /// <returns></returns>
        public static Func<T1, T2, T3, R> Before<T1, T2, T3, T4, R>(this Func<T1, T2, T3, T4> before, Func<T4, R> after)
            => (_1, _2, _3) => after(before(_1, _2, _3));

        /// <summary>
        /// Inverse function composition. 
        /// </summary>
        /// <param name="before">This is done first</param>
        /// <param name="after">This is done after the first</param>
        /// <returns></returns>
        public static Func<T1, T2, T3, T4, R> Before<T1, T2, T3, T4, T5, R>(this Func<T1, T2, T3, T4, T5> before, Func<T5, R> after)
            => (_1, _2, _3, _4) => after(before(_1, _2, _3, _4));

        /// <summary>
        /// Inverse function composition. 
        /// </summary>
        /// <param name="before">This is done first</param>
        /// <param name="after">This is done last</param>
        /// <returns></returns>
        public static Func<T1, T2, T3, T4, T5, R> Before<T1, T2, T3, T4, T5, T6, R>(this Func<T1, T2, T3, T4, T5, T6> before, Func<T6, R> after)
            => (_1, _2, _3, _4, _5) => after(before(_1, _2, _3, _4, _5));

        /// <summary>
        /// Function composition
        /// </summary>
        /// <param name="after">This is done last</param>
        /// <param name="before">This is done first</param>
        /// <returns></returns>
        public static Func<T1, R> After<T1, T2, R>(this Func<T2, R> after, Func<T1, T2> before)
            => _ => after(before(_));

        /// <summary>
        /// Function composition
        /// </summary>
        /// <param name="after">This is done last</param>
        /// <param name="before">This is done first</param>
        /// <returns></returns>
        public static Func<T1, T2, R> After<T1, T2, T3, R>(this Func<T3, R> after, Func<T1, T2, T3> before)
            => (_1, _2) => after(before(_1, _2));

        /// <summary>
        /// Function composition
        /// </summary>
        /// <param name="after">This is done last</param>
        /// <param name="before">This is done first</param>
        /// <returns></returns>
        public static Func<T1, T2, T3, R> After<T1, T2, T3, T4, R>(this Func<T4, R> after, Func<T1, T2, T3, T4> before)
            => (_1, _2, _3) => after(before(_1, _2, _3));

        /// <summary>
        /// Function composition
        /// </summary>
        /// <param name="after">This is done last</param>
        /// <param name="before">This is done first</param>
        /// <returns></returns>
        public static Func<T1, T2, T3, T4, R> After<T1, T2, T3, T4, T5, R>(this Func<T5, R> after, Func<T1, T2, T3, T4, T5> before)
            => (_1, _2, _3, _4) => after(before(_1, _2, _3, _4));

        /// <summary>
        /// Function composition
        /// </summary>
        /// <param name="after">This is done last</param>
        /// <param name="before">This is done first</param>
        /// <returns></returns>
        public static Func<T1, T2, T3, T4, T5, R> After<T1, T2, T3, T4, T5, T6, R>(this Func<T6, R> after, Func<T1, T2, T3, T4, T5, T6> before)
            => (_1, _2, _3, _4, _5) => after(before(_1, _2, _3, _4, _5));


    }
}
