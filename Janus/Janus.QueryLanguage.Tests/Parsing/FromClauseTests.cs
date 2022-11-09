using Antlr4.Runtime.Tree;
using Antlr4.Runtime;

namespace Janus.QueryLanguage.Tests.Parsing;
public class FromClauseTests
{
    [Theory(DisplayName = "Parse FROM clause")]
    [InlineData("FROM datasource.schema.tableau")]
    [InlineData("FROM datasource1.schema1.tableau1 " +
                "JOIN datasource1.schema1.tableau2 " +
                    "ON datasource1.schema1.tableau1.attribute1 == datasource1.schema1.tableau2.attribute1")]
    [InlineData("FROM datasource1.schema1.tableau1 " +
                "JOIN datasource1.schema1.tableau2 " +
                    "ON datasource1.schema1.tableau1.attribute1 == datasource1.schema1.tableau2.attribute1 " +
                "JOIN datasource1.schema1.tableau3 " +
                    "ON datasource1.schema1.tableau2.attribute1 == datasource1.schema1.tableau3.attribute1")]
    public void ParseFromClause(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        QueryLanguageLexer lexer = new QueryLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        QueryLanguageParser parser = new QueryLanguageParser(commonTokenStream);

        QueryLanguageBaseListener parseListener = new QueryLanguageBaseListener();
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var fromContext = parser.from_clause();
        // ParseTreeWalker.Default.Walk(parseListener, fromContext);

        Assert.Empty(errorListener.Errors);
    }

    [Theory(DisplayName = "Fail to parse FROM clause")]
    [InlineData("FROM datasource.schema.tableau.attr1")]
    [InlineData("FROM datasource1.schema1.tableau1 " +
                "JOIN datasource1.schema1.tableau2 " +
                    "ON datasource1.schema1 == datasource1.schema1.tableau2.attribute1")]
    [InlineData("FROM datasource1.schema1.tableau1 " +
            "JOIN datasource1.schema1.tableau2 " +
                "ON datasource1.schema1.tableau1.attribute1 == datasource1.schema1.tableau2 " +
            "JOIN datasource1.schema1.tableau3 " +
                "ON datasource1.schema1.tableau2.attribute1 == datasource1.schema1.tableau3.attribute1")]
    public void FailParseFromClause(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        QueryLanguageLexer lexer = new QueryLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        QueryLanguageParser parser = new QueryLanguageParser(commonTokenStream);

        QueryLanguageBaseListener parseListener = new QueryLanguageBaseListener();
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var fromContext = parser.from_clause();
        // ParseTreeWalker.Default.Walk(parseListener, fromContext);

        Assert.NotEmpty(errorListener.Errors);
    }
}
