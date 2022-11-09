using Antlr4.Runtime.Tree;
using Antlr4.Runtime;

namespace Janus.QueryLanguage.Tests.Parsing;
public class SelectionExpressionsTests
{
    [Theory(DisplayName = "Parse a NOT selection expression")]
    [InlineData("NOT(datasource.schema.tableau.attr1 > 0)")]
    [InlineData("NOT(true)")]
    public void ParseNotExpressionTest(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        QueryLanguageLexer lexer = new QueryLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        QueryLanguageParser parser = new QueryLanguageParser(commonTokenStream);

        QueryLanguageBaseListener parseListener = new QueryLanguageBaseListener();
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var selectionExprCtx = parser.selection_expr();
        // ParseTreeWalker.Default.Walk(parseListener, selectionExprCtx);

        Assert.NotNull(selectionExprCtx.NOT_OP());
        Assert.Empty(errorListener.Errors);
    }

    [Theory(DisplayName = "Parse an AND selection expression")]
    [InlineData("datasource.schema.tableau.attr1 > 0 AND datasource.schema.tableau.attr2 == 0")]
    [InlineData("true AND FALSE")]
    public void ParseAndExpressionTest(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        QueryLanguageLexer lexer = new QueryLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        QueryLanguageParser parser = new QueryLanguageParser(commonTokenStream);

        QueryLanguageBaseListener parseListener = new QueryLanguageBaseListener();
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var selectionExprCtx = parser.selection_expr();
        // ParseTreeWalker.Default.Walk(parseListener, selectionExprCtx);

        Assert.NotNull(selectionExprCtx.AND_OP());
        Assert.Empty(errorListener.Errors);
    }

    [Theory(DisplayName = "Parse an OR selection expression")]
    [InlineData("datasource.schema.tableau.attr1 > 0 OR datasource.schema.tableau.attr2 == 0")]
    [InlineData("true OR FALSE")]
    public void ParseOrExpressionTest(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        QueryLanguageLexer lexer = new QueryLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        QueryLanguageParser parser = new QueryLanguageParser(commonTokenStream);

        QueryLanguageBaseListener parseListener = new QueryLanguageBaseListener();
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var selectionExprCtx = parser.selection_expr();
        // ParseTreeWalker.Default.Walk(parseListener, selectionExprCtx);

        Assert.NotNull(selectionExprCtx.OR_OP());
        Assert.Empty(errorListener.Errors);
    }

    [Theory(DisplayName = "Parse a mixed selection expression")]
    [InlineData("(datasource.schema.tableau.attr1 > 0 OR datasource.schema.tableau.attr2 == 0) AND TRUE")]
    [InlineData("NOT(false) AND true OR FALSE")]
    public void ParseSelectionExpressionTest(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        QueryLanguageLexer lexer = new QueryLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        QueryLanguageParser parser = new QueryLanguageParser(commonTokenStream);

        QueryLanguageBaseListener parseListener = new QueryLanguageBaseListener();
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var selectionExprCtx = parser.selection_expr();
        // ParseTreeWalker.Default.Walk(parseListener, selectionExprCtx);

        Assert.Empty(errorListener.Errors);
    }
}
