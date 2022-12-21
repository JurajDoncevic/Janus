using Janus.Mediator;
using Janus.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Components.IntegrationTests;
internal static class ComponentOptions
{
    internal static IEnumerable<WrapperOptions> WrapperOptions => Enumerable.Empty<WrapperOptions>();
    internal static IEnumerable<MediatorOptions> MediatorOptions => Enumerable.Empty<MediatorOptions>();
}
