using FunctionalExtensions.Base.Resulting;

namespace Janus.Wrapper.Sqlite.ConsoleApp.Displays;
public abstract class BaseDisplay
{
    protected SqliteWrapperManager _wrapperController;
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

    protected BaseDisplay(SqliteWrapperManager wrapperController)
    {
        _wrapperController = wrapperController ?? throw new ArgumentNullException(nameof(wrapperController));
    }
}
