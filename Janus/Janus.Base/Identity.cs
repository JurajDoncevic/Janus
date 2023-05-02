using System;
using System.Collections.Generic;
using System.Text;

namespace Janus.Base
{
    /// <summary>
    /// Identity functor
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct Identity<T>
    {
        private readonly T _data;
        
        public T Data => _data;

        internal Identity(T data)
        {
            _data = data;
        }

    }

    public static class IdentityExtensions
    {
        /// <summary>
        /// Return function for the Identity functor
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">Data to be wrapped</param>
        /// <returns></returns>
        public static Identity<T> Identity<T>(this T data) 
            => new Identity<T>(data);

        /// <summary>
        /// Identity functor map function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="target"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Identity<R> Map<T, R>(this Identity<T> target, Func<T, R> func) 
            => new Identity<R>(func(target.Data));
    }
}
