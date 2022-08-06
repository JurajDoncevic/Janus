using FunctionalExtensions.Base.Results;
using Janus.Wrapper.Sqlite;

namespace Janus.Wrapper.Sqlite.ConsoleApp.Displays;
public abstract class BaseDisplay
{
    protected SqliteWrapperController _wrapperController;
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

    protected BaseDisplay(SqliteWrapperController wrapperController!!)
    {
        _wrapperController = wrapperController;
    }
}
