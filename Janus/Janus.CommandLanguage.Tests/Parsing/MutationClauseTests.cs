using Antlr4.Runtime.Tree;
using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.CommandLanguage.Tests.Parsing;
public class MutationClauseTests
{
    [Theory(DisplayName = "Parse a MUTATION clause")]
    [InlineData("WITH attr1 <- 1, attr2 <- \"some string\", attr3 <- 3.14, attr4 <- 30-08-2022T12:59:59")]
    [InlineData("WITH attr1 <- 1, attr2 <- \"some string\", attr3 <- 3.00, attr4 <- true")]
    [InlineData("WITH attr1 <- 1, attr2 <- \"some string\", attr3 <- 3.00, attr4 <- FALSE")]
    [InlineData("WITH attr1 <- 1, attr2 <- \"some string\"")]
    [InlineData("WITH attr1 <- 1")]
    public void ParseMutationClauseTest(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        CommandLanguageLexer lexer = new CommandLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        CommandLanguageParser parser = new CommandLanguageParser(commonTokenStream);

        CommandLanguageBaseListener parseListener = new CommandLanguageBaseListener();
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var mutationClause = parser.mutation_clause();
        // ParseTreeWalker.Default.Walk(parseListener, mutationClause);

        Assert.Empty(errorListener.Errors);
    }
}
