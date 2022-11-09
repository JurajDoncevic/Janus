using Antlr4.Runtime.Tree;
using Antlr4.Runtime;

namespace Janus.MediationLanguage.Tests.Parsing;
public class MediationClausesTests
{
    [Theory(DisplayName = "Parse datasource mediation")]
    [InlineData("DATASOURCE testDataSource")]
    [InlineData("DATASOURCE testDataSource VERSION \"v1.1\" #this is a datasource#")]
    public void ParseDataSourceMediation(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        MediationLanguageLexer lexer = new MediationLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        MediationLanguageParser parser = new MediationLanguageParser(commonTokenStream);

        MediationLanguageBaseListener parseListener = new MediationLanguageBaseListener();
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var mediation = parser.datasource_mediation();
        ParseTreeWalker.Default.Walk(parseListener, mediation);

        Assert.Empty(errorListener.Errors);
        Assert.NotNull(mediation.STRUCTURE_NAME());
        if (testText.Contains("VERSION"))
        {
            Assert.NotNull(mediation.version_expr().STRING());
        }
        if (testText.Contains("#"))
        {
            Assert.NotNull(mediation.description_expr().DESCRIPTION_TEXT());
        }
    }

    [Theory(DisplayName = "Parse schema mediation")]
    [InlineData("WITH SCHEMA testSchema")]
    [InlineData("WITH SCHEMA testSchema #this is a schema description#")]
    public void ParseSchemaMediation(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        MediationLanguageLexer lexer = new MediationLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        MediationLanguageParser parser = new MediationLanguageParser(commonTokenStream);

        MediationLanguageBaseListener parseListener = new MediationLanguageBaseListener();
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var mediation = parser.schema_mediation();
        ParseTreeWalker.Default.Walk(parseListener, mediation);

        Assert.Empty(errorListener.Errors);
        Assert.NotNull(mediation.STRUCTURE_NAME());
        if (testText.Contains("#"))
        {
            Assert.NotNull(mediation.description_expr().DESCRIPTION_TEXT());
        }
    }

    [Theory(DisplayName = "Parse destination clause in tableau mediation")]
    [InlineData("WITH ATTRIBUTES " +
                    "id #this is an ID#, " +
                    "firstName #this is the person's first name#, " +
                    "lastName #this is the person's last name#, " +
                    "dateOfBirth #this is the person's date of birth#")]
    public void ParseDestinationClause(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        MediationLanguageLexer lexer = new MediationLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        MediationLanguageParser parser = new MediationLanguageParser(commonTokenStream);

        MediationLanguageBaseListener parseListener = new MediationLanguageBaseListener();
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var clause = parser.attribute_mediation();
        ParseTreeWalker.Default.Walk(parseListener, clause);

        Assert.Empty(errorListener.Errors);
        Assert.NotNull(clause);
        Assert.Equal(4, clause.attribute_declaration().Count());

    }

    [Theory(DisplayName = "Parse source query clause in tableau mediation")]
    [InlineData("SELECT ds1.sc1.tb1.attr1, ds1.sc1.tb1.attr2 " +
                "FROM ds1.sc1.tb1")]
    [InlineData("SELECT ds1.sc1.tb1.attr1, ds1.sc1.tb1.attr2, ds1.sc1.tb2.attr1, ds1.sc1.tb2.attr1 " +
                "FROM ds1.sc1.tb1 JOIN ds1.sc1.tb2 ON ds1.sc1.tb1.attr1 == ds1.sc1.tb2.attr1")]
    public void ParseSourceQueryClause(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        MediationLanguageLexer lexer = new MediationLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        MediationLanguageParser parser = new MediationLanguageParser(commonTokenStream);

        MediationLanguageBaseListener parseListener = new MediationLanguageBaseListener();
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var clause = parser.source_query_clause();
        ParseTreeWalker.Default.Walk(parseListener, clause);
        Assert.Empty(errorListener.Errors);
        Assert.NotNull(clause.select_clause());
        Assert.NotNull(clause.from_clause());
    }

    [Theory(DisplayName = "Parse tableau mediation")]
    [InlineData("WITH TABLEAU testTableau " +
                "WITH ATTRIBUTES id, firstName, lastName, dateOfBirth BEING " +
                "SELECT ds.sc.tb1.id, ds.sc.tb1.firstName, ds.sc.tb1.lastName, ds.sc.tb2.dateOfBirth " +
                "FROM ds.sc.tb1 JOIN ds.sc.tb2 ON ds.sc.tb1.id == dc.sc.tb2.id")]
    public void ParseTableauMediation(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        MediationLanguageLexer lexer = new MediationLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        MediationLanguageParser parser = new MediationLanguageParser(commonTokenStream);

        MediationLanguageBaseListener parseListener = new MediationLanguageBaseListener();
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var mediation = parser.tableau_mediation();
        ParseTreeWalker.Default.Walk(parseListener, mediation);
        Assert.Empty(errorListener.Errors);
        if (testText.Contains("ATTRIBUTES"))
        {
            Assert.NotNull(mediation.attribute_mediation());
            Assert.NotNull(mediation.source_query_clause());
        }

    }
}
