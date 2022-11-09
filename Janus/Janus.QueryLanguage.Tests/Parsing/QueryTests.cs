using Antlr4.Runtime.Tree;
using Antlr4.Runtime;

namespace Janus.QueryLanguage.Tests.Parsing;
public class QueryTests
{
    [Theory(DisplayName = "Parse query")]
    [InlineData("SELECT datasource1.schema2.tableau1.attr1, datasource1.schema2.tableau1.attr2, datasource1.schema2.tableau1.attr3 " +
                "FROM datasource1.schema1.tableau1 " +
                "JOIN datasource1.schema1.tableau2 " +
                    "ON datasource1.schema1.tableau1.attribute1 == datasource1.schema1.tableau2.attribute1 " +
                "WHERE datasource.schema.tableau.attr1 > 0 AND datasource.schema.tableau.attr2 == 0;")]
    [InlineData("SELECT * " +
                "FROM datasource1.schema1.tableau1 " +
                "JOIN datasource1.schema1.tableau2 " +
                    "ON datasource1.schema1.tableau1.attribute1 == datasource1.schema1.tableau2.attribute1 " +
                "WHERE (datasource.schema.tableau.attr1 > 0 OR datasource.schema.tableau.attr2 == 0) AND TRUE;")]
    [InlineData("SELECT datasource1.schema2.tableau1.attr1, datasource1.schema2.tableau1.attr2, datasource1.schema2.tableau1.attr3 " +
                "FROM datasource1.schema1.tableau1 " +
                "WHERE TRUE;")]
    public void ParseQueryTest(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        QueryLanguageLexer lexer = new QueryLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        QueryLanguageParser parser = new QueryLanguageParser(commonTokenStream);

        QueryLanguageBaseListener parseListener = new QueryLanguageBaseListener();
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var query = parser.query();
        // ParseTreeWalker.Default.Walk(parseListener, query);

        Assert.Empty(errorListener.Errors);
    }
}
