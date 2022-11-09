using Antlr4.Runtime.Tree;
using Antlr4.Runtime;

namespace Janus.CommandLanguage.Tests.Parsing;
public class InstantiationClauseTests
{
    [Theory(DisplayName = "Parse a INSTANTIATION clause")]
    [InlineData("VALUES (1, \"some string\", 3.14, 30-08-2022T12:59:59)")]
    [InlineData("VALUES (1, \"some string\", 3.00, 30-08-2022T12:59:59)")]
    [InlineData("VALUES (TRUE)")]
    [InlineData("VALUES (1, \"some string\", FALSE)")]
    public void ParseInstantiationClauseTest(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        CommandLanguageLexer lexer = new CommandLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        CommandLanguageParser parser = new CommandLanguageParser(commonTokenStream);

        CommandLanguageBaseListener parseListener = new CommandLanguageBaseListener();
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var instantiationClause = parser.instantiation_clause();
        // ParseTreeWalker.Default.Walk(parseListener, instantiationClause);

        Assert.Empty(errorListener.Errors);
    }
}
