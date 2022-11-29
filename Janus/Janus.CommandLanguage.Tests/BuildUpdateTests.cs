using Antlr4.Runtime.Tree;
using Antlr4.Runtime;
using Janus.Commons.SelectionExpressions;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.CommandLanguage.Tests;
public class BuildUpdateTests
{

    [Theory(DisplayName = "Build UPDATE command")]
    [InlineData("UPDATE datasource1.schema1.tableau1 " +
                "WITH attr1 <- 1, attr2 <- \"some string\", attr3 <- 3.14 " +
                "WHERE (datasource.schema.tableau.attr1 > 0 OR datasource.schema.tableau.attr2 == 0) AND TRUE;")]
    [InlineData("UPDATE datasource1.schema1.tableau1 " +
                "WITH attr1 <- 1, attr2 <- \"some string\", attr3 <- 3.00 " +
                "WHERE (datasource.schema.tableau.attr1 > 0 OR datasource.schema.tableau.attr2 == 0) AND TRUE;")]
    public void BuildUpdateCommand(string testText)
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

        var builtCommandResult = parseListener.BuildUpdateCommand();
        var updateCommand = builtCommandResult.Data;

        Assert.Empty(errorListener.Errors);
        Assert.True(builtCommandResult);
        Assert.True(updateCommand?.Selection);
        Assert.NotNull(updateCommand?.Mutation);
        Assert.NotNull(updateCommand?.OnTableauId);
    }
}
