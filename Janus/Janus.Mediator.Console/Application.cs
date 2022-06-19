using Janus.Mediator.Core;

namespace Janus.Mediator.Console;
public class Application
{
    private readonly MediatorController _mediatorController;

    public Application(MediatorController mediatorController, MediatorOptions mediatorOptions)
    {
        _mediatorController = mediatorController;
        RunWelcome();
    }

    public Task RunWelcome()
    {
        System.Console.WriteLine("Welcome to the mediator cli application! This duck is it for now, press any key...");
        System.Console.WriteLine(
            @"    __
___( o)>
\ <_. )
 `---'   "
            );
        System.Console.ReadKey();
        return Task.CompletedTask;
    }
}
