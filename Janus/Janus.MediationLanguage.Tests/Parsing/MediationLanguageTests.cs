using Antlr4.Runtime.Tree;
using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.MediationLanguage.Tests.Parsing;
public class MediationLanguageTests
{
    [Theory(DisplayName = "Parse mediation script")]

    [InlineData(
        "DATASOURCE HumanResources VERSION \"1.0\" #Mediated human resources data from multiple sources# " +
            "WITH SCHEMA PublicSchema #Default schema about human resources#" +
                "WITH TABLEAU People #Data about people#" +
                    "WITH ATTRIBUTES " +
                        "Id #Person identifier#, " +
                        "FirstName #Person's first name#, " +
                        "LastName #Person's last name#, " +
                        "DoB #Person's date of birth# " +
                    "BEING " +
                        "SELECT enterprise1.public.people.id, enterprise1.public.people.first_name, enterprise1.public.people.last_name, enterprise2.public.people_births.dob " +
                        "FROM enterprise1.public.people JOIN enterprise2.public.people_births ON enterprise1.public.people.id == enterprise2.public.people_births.person_id " +
                "WITH TABLEAU Departments #Data about departments# " +
                    "WITH ATTRIBUTES " +
                        "Id #Department identifier#, " +
                        "Name #Department name# " +
                    "BEING " +
                        "SELECT enterprise3.public.departments.id_department, enterprise3.public.departments.deparment_name " +
                        "FROM enterprise3.public.departments " +
            "WITH SCHEMA EmptySchema #This is an empty schema#"
        )]

    public void ParseMediation(string testText)
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
        Assert.NotNull(mediation);
    }
}
