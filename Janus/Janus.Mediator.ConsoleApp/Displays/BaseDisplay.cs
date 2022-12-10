using FunctionalExtensions.Base.Resulting;

namespace Janus.Mediator.ConsoleApp.Displays;
public abstract class BaseDisplay
{
    protected MediatorManager _mediatorController;
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

    protected BaseDisplay(MediatorManager MediatorManager)
    {
        _mediatorController = MediatorManager ?? throw new ArgumentNullException(nameof(MediatorManager));
    }
}
