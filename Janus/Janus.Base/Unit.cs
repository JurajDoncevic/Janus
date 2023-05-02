using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Janus.Base
{
    /// <summary>
    /// Terminal data type. Signifier for functions that don't return anything. Functions returning this have side-effects!
    /// </summary>
    public struct Unit { };

    public static partial class UnitExtensions
    {   
        /// <summary>
        /// Signifier for functions that don't return anything. Functions returning this have side-effects!
        /// </summary>
        /// <returns>ValueTuple masked as Unit</returns>
        public static Unit Unit() => default(Unit);


        /// <summary>
        /// Used to ignore the result of a previous function - converts any type of data to Unit
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Unit Ignore<T>(this T target) => Unit();
    }

    /// <summary>
    /// Extensions to convert Actions to Funcs
    /// </summary>
    public static class ActionExtensions
    {
        public static Func<Unit> ToFunc(this Action action) =>
            () => { action(); return UnitExtensions.Unit(); };

        public static Func<T, Unit> ToFunc<T>(this Action<T> action) =>
            (_1) => { action(_1); return UnitExtensions.Unit(); };

        public static Func<T1, T2, Unit> ToFunc<T1, T2>(this Action<T1, T2> action) =>
            (_1, _2) => { action(_1, _2); return UnitExtensions.Unit(); };

        public static Func<T1, T2, T3, Unit> ToFunc<T1, T2, T3>(this Action<T1, T2, T3> action) =>
            (_1, _2, _3) => { action(_1, _2, _3); return UnitExtensions.Unit(); };

        public static Func<T1, T2, T3, T4, Unit> ToFunc<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action) =>
            (_1, _2, _3, _4) => { action(_1, _2, _3, _4); return UnitExtensions.Unit(); };

        public static Func<T1, T2, T3, T4, T5, Unit> ToFunc<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action) =>
            (_1, _2, _3, _4, _5) => { action(_1, _2, _3, _4, _5); return UnitExtensions.Unit(); };

        public static Func<T1, T2, T3, T4, T5, T6, Unit> ToFunc<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action) =>
            (_1, _2, _3, _4, _5, _6) => { action(_1, _2, _3, _4, _5, _6); return UnitExtensions.Unit(); };

        public static Func<T1, T2, T3, T4, T5, T6, T7, Unit> ToFunc<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action) =>
            (_1, _2, _3, _4, _5, _6, _7) => { action(_1, _2, _3, _4, _5, _6, _7); return UnitExtensions.Unit(); };

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, Unit> ToFunc<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action) =>
            (_1, _2, _3, _4, _5, _6, _7, _8) => { action(_1, _2, _3, _4, _5, _6, _7, _8); return UnitExtensions.Unit(); };

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Unit> ToFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action) =>
            (_1, _2, _3, _4, _5, _6, _7, _8, _9) => { action(_1, _2, _3, _4, _5, _6, _7, _8, _9); return UnitExtensions.Unit(); };

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Unit> ToFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action) =>
            (_1, _2, _3, _4, _5, _6, _7, _8, _9, _10) => { action(_1, _2, _3, _4, _5, _6, _7, _8, _9, _10); return UnitExtensions.Unit(); };

    }
}
