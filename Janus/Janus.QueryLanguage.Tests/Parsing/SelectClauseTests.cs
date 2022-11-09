using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using static System.Net.Mime.MediaTypeNames;

namespace Janus.QueryLanguage.Tests.Parsing;
public class SelectClauseTests
{
    [Theory(DisplayName = "Parse SELECT with *")]
    [InlineData("SELECT *")]
    [InlineData("SELECT datasource1.schema2.tableau1.attr1")]
    [InlineData("SELECT datasource1.schema2.tableau1.attr1, datasource1.schema2.tableau1.attr2, datasource1.schema2.tableau1.attr3")]
    public void ParseSelectClauseTest(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        QueryLanguageLexer lexer = new QueryLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        QueryLanguageParser parser = new QueryLanguageParser(commonTokenStream);

        QueryLanguageBaseListener parseListener = new QueryLanguageBaseListener();
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var selectContext = parser.select_clause();
        // ParseTreeWalker.Default.Walk(parseListener, selectContext);

        Assert.Empty(errorListener.Errors);
    }

    [Theory(DisplayName = "Fail to parse SELECT")]
    [InlineData("SELECT datasource1.schema2.tableau1, datasource1.schema2.tableau1, datasource1.schema2.tableau1")]
    public void FaileParseSelectClauseWithTableausTest(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        QueryLanguageLexer lexer = new QueryLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        QueryLanguageParser parser = new QueryLanguageParser(commonTokenStream);

        QueryLanguageBaseListener parseListener = new QueryLanguageBaseListener();
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var selectContext = parser.select_clause();
        // ParseTreeWalker.Default.Walk(parseListener, selectContext);

        Assert.NotEmpty(errorListener.Errors);
    }
}
