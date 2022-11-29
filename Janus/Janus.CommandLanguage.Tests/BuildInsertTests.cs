using Antlr4.Runtime.Tree;
using Antlr4.Runtime;
using Janus.Commons.SelectionExpressions;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.CommandLanguage.Tests;
public class BuildInsertTests
{

    [Theory(DisplayName = "Build INSERT command")]
    [InlineData("INSERT " +
                "INTO datasource1.schema1.tableau1(attr1, attr2, attr3) " +
                "VALUES (1, \"some string\", 3.14);")]
    [InlineData("INSERT " +
                "INTO datasource1.schema1.tableau1(attr1, attr2, attr3) " +
                "VALUES (1, \"some string\", 3.14) (2, \"some string 2\", 3.00);")]
    public void BuildInsertCommand(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        CommandLanguageLexer lexer = new CommandLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        CommandLanguageParser parser = new CommandLanguageParser(commonTokenStream);

        CommandLanguageListener parseListener = new CommandLanguageListener();
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var command = parser.command();

        var builtCommandResult = parseListener.BuildInsertCommand();
        var insertCommand = builtCommandResult.Data;

        Assert.Empty(errorListener.Errors);
        Assert.True(builtCommandResult);
        Assert.NotNull(insertCommand?.Instantiation);
        Assert.NotNull(insertCommand?.OnTableauId);
    }
}
