using Antlr4.Runtime.Tree;
using Antlr4.Runtime;

namespace Janus.CommandLanguage.Tests.Parsing;
public class ComparisonExpressionsTests
{
    [Theory(DisplayName = "Parse GT expression")]
    [InlineData("datasource.schema.tableau.attribute > 23")]
    public void ParseGreaterThanExpression(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        CommandLanguageLexer lexer = new CommandLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        CommandLanguageParser parser = new CommandLanguageParser(commonTokenStream);

        CommandLanguageBaseListener parseListener = new CommandLanguageBaseListener();
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var ctx = parser.gt_expr();
        // ParseTreeWalker.Default.Walk(parseListener, ctx);

        Assert.Empty(errorListener.Errors);
    }

    [Theory(DisplayName = "Parse GTE expression")]
    [InlineData("datasource.schema.tableau.attribute >= 23")]
    public void ParseGreaterThanOrEqualsExpression(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        CommandLanguageLexer lexer = new CommandLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        CommandLanguageParser parser = new CommandLanguageParser(commonTokenStream);

        CommandLanguageBaseListener parseListener = new CommandLanguageBaseListener();
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var ctx = parser.gte_expr();
        // ParseTreeWalker.Default.Walk(parseListener, ctx);

        Assert.Empty(errorListener.Errors);
    }

    [Theory(DisplayName = "Parse LT expression")]
    [InlineData("datasource.schema.tableau.attribute < 23")]
    public void ParseLesserThanExpression(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        CommandLanguageLexer lexer = new CommandLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        CommandLanguageParser parser = new CommandLanguageParser(commonTokenStream);

        CommandLanguageBaseListener parseListener = new CommandLanguageBaseListener();
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var ctx = parser.lt_expr();
        // ParseTreeWalker.Default.Walk(parseListener, ctx);

        Assert.Empty(errorListener.Errors);
    }

    [Theory(DisplayName = "Parse LTE expression")]
    [InlineData("datasource.schema.tableau.attribute <= 23")]
    public void ParseLesserThanOrEqualsExpression(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        CommandLanguageLexer lexer = new CommandLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        CommandLanguageParser parser = new CommandLanguageParser(commonTokenStream);

        CommandLanguageBaseListener parseListener = new CommandLanguageBaseListener();
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var ctx = parser.lte_expr();
        // ParseTreeWalker.Default.Walk(parseListener, ctx);

        Assert.Empty(errorListener.Errors);
    }

    [Theory(DisplayName = "Parse EQ expression")]
    [InlineData("datasource.schema.tableau.attribute == 23")]
    public void ParseQeualsExpression(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        CommandLanguageLexer lexer = new CommandLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        CommandLanguageParser parser = new CommandLanguageParser(commonTokenStream);

        CommandLanguageBaseListener parseListener = new CommandLanguageBaseListener();
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var ctx = parser.eq_expr();
        // ParseTreeWalker.Default.Walk(parseListener, ctx);

        Assert.Empty(errorListener.Errors);
    }

    [Theory(DisplayName = "Parse NEQ expression")]
    [InlineData("datasource.schema.tableau.attribute != 23")]
    public void ParseLesserNotEqualsExpression(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        CommandLanguageLexer lexer = new CommandLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        CommandLanguageParser parser = new CommandLanguageParser(commonTokenStream);

        CommandLanguageBaseListener parseListener = new CommandLanguageBaseListener();
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var ctx = parser.neq_expr();
        // ParseTreeWalker.Default.Walk(parseListener, ctx);

        Assert.Empty(errorListener.Errors);
    }
}
