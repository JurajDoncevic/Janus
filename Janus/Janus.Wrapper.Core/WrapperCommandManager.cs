using FunctionalExtensions.Base.Results;
using Janus.Commons.CommandModels;
using Janus.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.Core;
public class WrapperCommandManager : IComponentCommandManager
{
    public Task<Result> RunCommand(BaseCommand command)
    {
        throw new NotImplementedException();
    }
}
