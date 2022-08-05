using FunctionalExtensions.Base.Results;

namespace Janus.Wrapper.ConsoleApp.Displays;
public abstract class BaseDisplay
{
    protected WrapperController _wrapperController;
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

    protected BaseDisplay(WrapperController mediatorController!!)
    {
        _wrapperController = mediatorController;
    }
}
