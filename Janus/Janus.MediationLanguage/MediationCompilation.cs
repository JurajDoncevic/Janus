using Antlr4.Runtime.Tree;
using Antlr4.Runtime;
using Janus.Mediation.SchemaMediationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Janus.Commons.SchemaModels;

namespace Janus.MediationLanguage;
public class MediationCompilation
{
    public static Result<DataSourceMediation> CompileMediationFromScriptText(string mediationScript, IEnumerable<DataSource> availableDataSources)
        => Results.AsResult(() =>
        {
            AntlrInputStream inputStream = new AntlrInputStream(mediationScript);
            MediationLanguageLexer lexer = new MediationLanguageLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            MediationLanguageParser parser = new MediationLanguageParser(commonTokenStream);

            MediationLanguageListener parseListener = new MediationLanguageListener(availableDataSources);
            VerboseErrorListener errorListener = new VerboseErrorListener();

            parser.AddParseListener(parseListener);
            parser.AddErrorListener(errorListener);

            var parsedMediation = parser.datasource_mediation();

            return parseListener.GenerateMediation();
        });
}
