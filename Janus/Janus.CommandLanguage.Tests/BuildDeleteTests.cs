using Antlr4.Runtime.Tree;
using Antlr4.Runtime;
using Janus.Commons.SelectionExpressions;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.CommandLanguage.Tests;
public class BuildDeleteTests
{

    [Theory(DisplayName = "Build DELETE command")]
    [InlineData("DELETE " +
                "FROM datasource1.schema1.tableau1 " +
                "WHERE TRUE;")]
    [InlineData("DELETE " +
                "FROM datasource1.schema1.tableau1 " +
                "WHERE datasource.schema.tableau.attr1 > 0 AND datasource.schema.tableau.attr2 == 0;")]
    [InlineData("DELETE " +
                "FROM datasource1.schema1.tableau1 " +
                "WHERE (datasource.schema.tableau.attr1 > 0 OR datasource.schema.tableau.attr2 == 0) AND TRUE;")]
    public void BuildDeleteCommand(string testText)
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

        var builtCommandResult = parseListener.BuildDeleteCommand();
        var deleteCommand = builtCommandResult.Data;

        Assert.Empty(errorListener.Errors);
        Assert.True(builtCommandResult);
        Assert.True(deleteCommand!.Selection);
        Assert.True(!string.IsNullOrEmpty(deleteCommand.OnTableauId.Trim()));
    }
}
