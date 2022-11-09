using Antlr4.Runtime.Tree;
using Antlr4.Runtime;

namespace Janus.QueryLanguage.Tests.Parsing;
public class LiteralsTests
{
    [Theory(DisplayName = "Tokenize DATETIME literal")]
    [InlineData("20-05-1999T11:00:00")]
    [InlineData("30-08-2022T12:59:59")]
    public void TokenizeDateTimeTest(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        QueryLanguageLexer lexer = new QueryLanguageLexer(inputStream);


        var testToken = lexer.GetAllTokens().First();
        var tokenName = lexer.Vocabulary.GetDisplayName(testToken.Type);
        Assert.Equal("DATETIME", tokenName);
    }


    [Theory(DisplayName = "Tokenize INTEGER literal")]
    [InlineData("42")]
    [InlineData("-42")]
    [InlineData("0")]
    public void TokenizeIntegerTest(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        QueryLanguageLexer lexer = new QueryLanguageLexer(inputStream);


        var testToken = lexer.GetAllTokens().First();
        var tokenName = lexer.Vocabulary.GetDisplayName(testToken.Type);
        Assert.Equal("INTEGER", tokenName);
    }

    [Theory(DisplayName = "Tokenize DECIMAL literal")]
    [InlineData("42.03240")]
    [InlineData("-42.03240")]
    [InlineData("0.0")]
    [InlineData("0.0324")]
    [InlineData("-0.0324")]
    public void TokenizeDecimalTest(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        QueryLanguageLexer lexer = new QueryLanguageLexer(inputStream);


        var testToken = lexer.GetAllTokens().First();
        var tokenName = lexer.Vocabulary.GetDisplayName(testToken.Type);
        Assert.Equal("DECIMAL", tokenName);
    }

    [Theory(DisplayName = "Tokenize BOOLEAN literal")]
    [InlineData("true")]
    [InlineData("TRUE")]
    [InlineData("false")]
    [InlineData("FALSE")]
    public void TokenizeBooleanTest(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        QueryLanguageLexer lexer = new QueryLanguageLexer(inputStream);


        var testToken = lexer.GetAllTokens().First();
        var tokenName = lexer.Vocabulary.GetDisplayName(testToken.Type);
        Assert.Equal("BOOLEAN", tokenName);
    }

    [Theory(DisplayName = "Tokenize STRING literal")]
    [InlineData("\"some string\"")]
    [InlineData("\"\"")]
    [InlineData("\"some\nstring\r\n\"")]
    public void TokenizeStringTest(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        QueryLanguageLexer lexer = new QueryLanguageLexer(inputStream);


        var testToken = lexer.GetAllTokens().First();
        var tokenName = lexer.Vocabulary.GetDisplayName(testToken.Type);
        Assert.Equal("STRING", tokenName);
    }

    [Theory(DisplayName = "Tokenize BINARY literal")]
    [InlineData($"0x0123456789ABCDEF")]
    [InlineData($"0xABCDEF0123456789")]
    [InlineData($"0xFEFEFEDADA")]
    [InlineData($"0x0023DFA")]
    [InlineData($"0x00F3DFA")]
    public void TokenizeBinaryTest(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        QueryLanguageLexer lexer = new QueryLanguageLexer(inputStream);


        var testToken = lexer.GetAllTokens().First();
        var tokenName = lexer.Vocabulary.GetDisplayName(testToken.Type);
        Assert.Equal("BINARY", tokenName);
    }

}
