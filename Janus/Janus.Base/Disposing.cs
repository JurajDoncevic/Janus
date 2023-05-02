using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Base
{
    public static class Disposing
    {
        public static TResult Using<TWith, TResult>(
                Func<TWith> setup,
                Func<TWith, TResult> operate)
            where TWith : IDisposable
        {
            using (var with = setup())
            {
                return operate(with);
            }
        }

        public async static Task<TResult> Using<TWith, TResult>(
                Func<TWith> setup,
                Func<TWith, Task<TResult>> operate)
            where TWith : IDisposable
        {
            using (var with = setup())
            {
                return await operate(with);
            }
        }
    }
}
