using Antlr4.Runtime.Tree;
using Antlr4.Runtime;

namespace Janus.QueryLanguage.Tests.Parsing;
public class WhereClauseTests
{

    [Theory(DisplayName = "Parse a Boolean WHERE clause")]
    [InlineData("WHERE true")]
    [InlineData("WHERE false")]
    [InlineData("WHERE (false)")]
    [InlineData("WHERE (true)")]
    public void ParseBoolWhereClauseTest(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        QueryLanguageLexer lexer = new QueryLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        QueryLanguageParser parser = new QueryLanguageParser(commonTokenStream);

        QueryLanguageBaseListener parseListener = new QueryLanguageBaseListener();
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var whereClause = parser.where_clause();
        // ParseTreeWalker.Default.Walk(parseListener, whereClause);

        Assert.NotNull(
            whereClause.selection_expr().BOOLEAN()
            ?? whereClause.selection_expr().selection_expr().FirstOrDefault()?.BOOLEAN());
        Assert.Empty(errorListener.Errors);
    }

    [Theory(DisplayName = "Parse a NOT WHERE clause")]
    [InlineData("WHERE NOT(datasource.schema.tableau.attr1 > 0)")]
    [InlineData("WHERE NOT(true)")]
    public void ParseNotWhereClauseTest(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        QueryLanguageLexer lexer = new QueryLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        QueryLanguageParser parser = new QueryLanguageParser(commonTokenStream);

        QueryLanguageBaseListener parseListener = new QueryLanguageBaseListener();
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var whereClause = parser.where_clause();
        // ParseTreeWalker.Default.Walk(parseListener, whereClause);

        Assert.NotNull(whereClause.selection_expr().NOT_OP());
        Assert.Empty(errorListener.Errors);
    }

    [Theory(DisplayName = "Parse an AND WHERE clause")]
    [InlineData("WHERE datasource.schema.tableau.attr1 > 0 AND datasource.schema.tableau.attr2 == 0")]
    [InlineData("WHERE true AND FALSE")]
    public void ParseAndWhereClauseTest(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        QueryLanguageLexer lexer = new QueryLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        QueryLanguageParser parser = new QueryLanguageParser(commonTokenStream);

        QueryLanguageBaseListener parseListener = new QueryLanguageBaseListener();
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var whereClause = parser.where_clause();
        // ParseTreeWalker.Default.Walk(parseListener, whereClause);

        Assert.NotNull(whereClause.selection_expr().AND_OP());
        Assert.Empty(errorListener.Errors);
    }

    [Theory(DisplayName = "Parse an OR WHERE clause")]
    [InlineData("WHERE datasource.schema.tableau.attr1 > 0 OR datasource.schema.tableau.attr2 == 0")]
    [InlineData("WHERE true OR FALSE")]
    public void ParseOrWhereClauseTest(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        QueryLanguageLexer lexer = new QueryLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        QueryLanguageParser parser = new QueryLanguageParser(commonTokenStream);

        QueryLanguageBaseListener parseListener = new QueryLanguageBaseListener();
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var whereClause = parser.where_clause();
        // ParseTreeWalker.Default.Walk(parseListener, whereClause);

        Assert.NotNull(whereClause.selection_expr().OR_OP());
        Assert.Empty(errorListener.Errors);
    }

    [Theory(DisplayName = "Parse a mixed WHERE clause")]
    [InlineData("WHERE (datasource.schema.tableau.attr1 > 0 OR datasource.schema.tableau.attr2 == 0) AND TRUE")]
    [InlineData("WHERE NOT(false) AND true OR FALSE")]
    public void ParseMixedWhereClauseTest(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        QueryLanguageLexer lexer = new QueryLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        QueryLanguageParser parser = new QueryLanguageParser(commonTokenStream);

        QueryLanguageBaseListener parseListener = new QueryLanguageBaseListener();
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var whereClause = parser.where_clause();
        // ParseTreeWalker.Default.Walk(parseListener, whereClause);

        Assert.Empty(errorListener.Errors);
    }
}
