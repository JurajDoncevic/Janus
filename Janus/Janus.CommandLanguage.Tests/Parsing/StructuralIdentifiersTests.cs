using Antlr4.Runtime;

namespace Janus.CommandLanguage.Tests.Parsing;
public class StructuralIdentifiersTests
{

    [Theory(DisplayName = "Tokenize SCHEMA identifier")]
    [InlineData("datasource.schema")]
    [InlineData("datasource1234.schema1234")]
    public void TokenizeSchemaIdentifierTest(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        CommandLanguageLexer lexer = new CommandLanguageLexer(inputStream);


        var testToken = lexer.GetAllTokens().First();
        var tokenName = lexer.Vocabulary.GetDisplayName(testToken.Type);
        Assert.Equal("SCHEMA_ID", tokenName);
    }

    [Theory(DisplayName = "Tokenize TABLEAU identifier")]
    [InlineData("datasource.schema.tableau")]
    [InlineData("datasource1234.schema1234.tableau123")]
    public void TokenizeTableauIdentifierTest(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        CommandLanguageLexer lexer = new CommandLanguageLexer(inputStream);


        var testToken = lexer.GetAllTokens().First();
        var tokenName = lexer.Vocabulary.GetDisplayName(testToken.Type);
        Assert.Equal("TABLEAU_ID", tokenName);
    }

    [Theory(DisplayName = "Tokenize ATTRIBUTE identifier")]
    [InlineData("datasource.schema.tableau.attr")]
    [InlineData("datasource1234.schema1234.tableau123.attr987")]
    public void TokenizeAttributeIdentifierTest(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        CommandLanguageLexer lexer = new CommandLanguageLexer(inputStream);


        var testToken = lexer.GetAllTokens().First();
        var tokenName = lexer.Vocabulary.GetDisplayName(testToken.Type);
        Assert.Equal("ATTRIBUTE_ID", tokenName);
    }
}
