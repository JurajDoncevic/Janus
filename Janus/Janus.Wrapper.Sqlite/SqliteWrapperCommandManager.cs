using Janus.Wrapper.LocalCommanding;
using Janus.Wrapper.Translation;

namespace Janus.Wrapper.Sqlite;
public class SqliteWrapperCommandManager : WrapperCommandManager<string, string, string>
{
    public SqliteWrapperCommandManager(ILocalCommandTranslator<LocalCommand, string, string, string> commandTranslator, ICommandExecutor<string, string, string> commandExecutor) : base(commandTranslator, commandExecutor)
    {
    }
}
