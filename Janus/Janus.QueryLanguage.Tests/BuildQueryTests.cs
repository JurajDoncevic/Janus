using Antlr4.Runtime.Tree;
using Antlr4.Runtime;
using Janus.Commons.SelectionExpressions;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.QueryLanguage.Tests;
public class BuildQueryTests
{

    [Theory(DisplayName = "Build query with SELECT and FROM clauses")]
    [InlineData("SELECT datasource1.schema1.tableau1.attr1, datasource1.schema1.tableau1.attr2, datasource1.schema1.tableau1.attr3 " +
                "FROM datasource1.schema1.tableau1 " +
                "WHERE TRUE;")]
    [InlineData("SELECT * " +
                "FROM datasource1.schema1.tableau1 " +
                "WHERE TRUE;")]
    [InlineData("SELECT datasource1.schema2.tableau1.attr1 " +
                "FROM datasource1.schema1.tableau1 " +
                "WHERE TRUE;")]
    public void BuildQueryWithFromAndSelect(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        QueryLanguageLexer lexer = new QueryLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        QueryLanguageParser parser = new QueryLanguageParser(commonTokenStream);

        QueryLanguageListener parseListener = new QueryLanguageListener();
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var query = parser.query();

        var builtQueryResult = parseListener.BuildQuery();

        Assert.Empty(errorListener.Errors);
        Assert.True(builtQueryResult);
    }

    [Theory(DisplayName = "Build query with SELECT, FROM, and WHERE clauses")]
    [InlineData("SELECT * " +
                "FROM datasource1.schema1.tableau1 " +
                "WHERE TRUE AND datasource1.schema1.tableau1.attr1 > 10;")]
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
    [InlineData("SELECT * " +
                "FROM datasource1.schema1.tableau1 " +
                "JOIN datasource1.schema1.tableau2 " +
                    "ON datasource1.schema1.tableau1.attribute1 == datasource1.schema1.tableau2.attribute1 " +
                "WHERE TRUE AND (datasource.schema.tableau.attr1 > 0 OR datasource.schema.tableau.attr2 == 0);")]
    [InlineData("SELECT datasource1.schema2.tableau1.attr1, datasource1.schema2.tableau1.attr2, datasource1.schema2.tableau1.attr3 " +
                "FROM datasource1.schema1.tableau1 " +
                "WHERE TRUE;")]
    public void BuildQueryWithWhere(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        QueryLanguageLexer lexer = new QueryLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        QueryLanguageParser parser = new QueryLanguageParser(commonTokenStream);

        QueryLanguageListener parseListener = new QueryLanguageListener();
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var query = parser.query();

        var builtQueryResult = parseListener.BuildQuery();

        Assert.Empty(errorListener.Errors);
        Assert.True(builtQueryResult);
        Assert.Equal(builtQueryResult.Data!.Selection.Value.Expression, TestSelectionExpressions[testText]);
    }

    // temporary solution
    private static Dictionary<string, SelectionExpression> TestSelectionExpressions
        => new Dictionary<string, SelectionExpression>
        {
            {
                "SELECT * " +
                "FROM datasource1.schema1.tableau1 " +
                "WHERE TRUE AND datasource1.schema1.tableau1.attr1 > 10;",
                AND(TRUE(), GT("datasource1.schema1.tableau1.attr1", 10))
            },
            {
                "SELECT datasource1.schema2.tableau1.attr1, datasource1.schema2.tableau1.attr2, datasource1.schema2.tableau1.attr3 " +
                "FROM datasource1.schema1.tableau1 " +
                "JOIN datasource1.schema1.tableau2 " +
                    "ON datasource1.schema1.tableau1.attribute1 == datasource1.schema1.tableau2.attribute1 " +
                "WHERE datasource.schema.tableau.attr1 > 0 AND datasource.schema.tableau.attr2 == 0;",
                AND(GT("datasource.schema.tableau.attr1", 0), EQ("datasource.schema.tableau.attr2", 0))
            },
            {
                "SELECT * " +
                "FROM datasource1.schema1.tableau1 " +
                "JOIN datasource1.schema1.tableau2 " +
                    "ON datasource1.schema1.tableau1.attribute1 == datasource1.schema1.tableau2.attribute1 " +
                "WHERE (datasource.schema.tableau.attr1 > 0 OR datasource.schema.tableau.attr2 == 0) AND TRUE;",
                AND(OR(GT("datasource.schema.tableau.attr1", 0), EQ("datasource.schema.tableau.attr2", 0)), TRUE())
            },
            {
                "SELECT * " +
                "FROM datasource1.schema1.tableau1 " +
                "JOIN datasource1.schema1.tableau2 " +
                    "ON datasource1.schema1.tableau1.attribute1 == datasource1.schema1.tableau2.attribute1 " +
                "WHERE TRUE AND (datasource.schema.tableau.attr1 > 0 OR datasource.schema.tableau.attr2 == 0);",
                AND(TRUE(), OR(GT("datasource.schema.tableau.attr1", 0), EQ("datasource.schema.tableau.attr2", 0)))
            },
            {
                "SELECT datasource1.schema2.tableau1.attr1, datasource1.schema2.tableau1.attr2, datasource1.schema2.tableau1.attr3 " +
                "FROM datasource1.schema1.tableau1 " +
                "WHERE TRUE;",
                TRUE()
            }
        };
}
