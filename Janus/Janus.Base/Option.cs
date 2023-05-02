using System;
using System.Collections.Generic;
using System.Text;

namespace Janus.Base
{
    /// <summary>
    /// Option class abstracts the existence of data
    /// </summary>
    /// <typeparam name="T">Abstracted data type</typeparam>
    public struct Option<T>
    {
        private readonly T _value;
        private readonly bool _isSome;

        /// <summary>
        /// Stored value
        /// </summary>
        public T Value => _value;

        /// <summary>
        /// Is there a stored value
        /// </summary>
        public bool IsSome => _isSome;

        internal Option(T value)
        {
            _value = value;
            _isSome = _value is { };
        }

        /// <summary>
        /// Creates an Option of a value: T -> O[T] 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Option<T> Some(T value) => new Option<T>(value);

        public override bool Equals(object obj)
        {
            return obj is Option<T> option &&
                _isSome == option._isSome &&
                EqualityComparer<T>.Default.Equals(_value, option._value);

        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_value, _isSome);
        }

        /// <summary>
        /// Creates a None Option: ()[T] -> None[T]
        /// </summary>
        public static Option<T> None => new Option<T>();

        public static implicit operator bool(Option<T> option)
        {
            return option.IsSome;
        }
    }

    public static class OptionExtensions
    {
        /// <summary>
        /// 
        /// Match if there is Some value or None and execute functions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="target"></param>
        /// <param name="onSomeFunc"></param>
        /// <param name="onNoneFunc"></param>
        /// <returns></returns>
        public static R Match<T, R>(this Option<T> target, Func<T, R> onSomeFunc, Func<R> onNoneFunc)
            where T : notnull
            where R : notnull
            => target.IsSome
                ? onSomeFunc(target.Value)
                : onNoneFunc();

        /// <summary>
        /// Bind operation for Option: O[T] -> (T -> O[R]) -> O[R] 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="target"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Option<R> Bind<T, R>(this Option<T> target, Func<T, Option<R>> func)
            where T : notnull
            where R : notnull
            => target.Match(
                _ => func(_),
                () => Option<R>.None
                );

        /// <summary>
        /// Map operation for Option: O[T] -> (T -> R) -> O[R]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="target"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Option<R> Map<T, R>(this Option<T> target, Func<T, R> func)
            where T : notnull
            where R : notnull
            => target.Match(
                _ => Option<R>.Some(func(_)),
                () => Option<R>.None
                );
    }
}


