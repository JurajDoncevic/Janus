using Antlr4.Runtime;
using Janus.Commons.CommandModels;

namespace Janus.CommandLanguage;
public class CommandCompilation
{
    public static Result<BaseCommand> CompileCommandFromScriptText(string commandText)
    => Results.AsResult(() =>
    {
        AntlrInputStream inputStream = new AntlrInputStream(commandText);
        CommandLanguageLexer lexer = new CommandLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        CommandLanguageParser parser = new CommandLanguageParser(commonTokenStream);

        CommandLanguageListener parseListener = new CommandLanguageListener();
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var command = parser.command();

        Result<BaseCommand> buildResult = parseListener switch
        {
            { ParsedCommandType.IsSome: true, ParsedCommandType.Value: CommandTypes.DELETE } => parseListener.BuildDeleteCommand().Map(_ => (BaseCommand)_),
            { ParsedCommandType.IsSome: true, ParsedCommandType.Value: CommandTypes.INSERT } => parseListener.BuildInsertCommand().Map(_ => (BaseCommand)_),
            { ParsedCommandType.IsSome: true, ParsedCommandType.Value: CommandTypes.UPDATE } => parseListener.BuildUpdateCommand().Map(_ => (BaseCommand)_),
            _ => Results.OnFailure<BaseCommand>("Unknown command type to build. The command might not have been parsed")
        };
        
        return buildResult;
    });
}
