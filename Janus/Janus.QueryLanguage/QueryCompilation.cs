using Antlr4.Runtime;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;

namespace Janus.QueryLanguage;
public class QueryCompilation
{
    public static Result<Query> CompileQueryFromScriptText(string queryText)
        => Results.AsResult(() =>
        {
            AntlrInputStream inputStream = new AntlrInputStream(queryText);
            QueryLanguageLexer lexer = new QueryLanguageLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            QueryLanguageParser parser = new QueryLanguageParser(commonTokenStream);

            QueryLanguageListener parseListener = new QueryLanguageListener();
            VerboseErrorListener errorListener = new VerboseErrorListener();

            parser.AddParseListener(parseListener);
            parser.AddErrorListener(errorListener);

            var query = parser.query();

            if (errorListener.HasErrors)
            {
                return Results.OnFailure<Query>($"Query parsing errors: {string.Join("\n", errorListener.Errors)}");
            }

            var buildResult = parseListener.BuildQuery();

            return  buildResult;
        });
}
