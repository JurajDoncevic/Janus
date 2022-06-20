using Janus.Mediator.Core;
using Janus.Utils.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Mediator.Console.Displays;
public abstract class BaseDisplay
{
    protected MediatorController _mediatorController;
    public abstract string Title { get; }
    protected abstract Task<Result> Display();

    protected virtual void PreDisplay()
    {
        System.Console.WriteLine($"--------{Title}--------");
    }

    protected virtual void PostDisplay()
    {
        System.Console.WriteLine();
    }

    public async Task<Result> Show()
    {
        PreDisplay();
        var result = await Display();
        PostDisplay();

        return result;
    }

    protected BaseDisplay(MediatorController mediatorController!!)
    {
        _mediatorController = mediatorController;
    }
}
