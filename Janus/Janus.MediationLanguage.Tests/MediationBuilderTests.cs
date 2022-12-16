using Antlr4.Runtime;
using Janus.Commons.SchemaModels;

namespace Janus.MediationLanguage.Tests;
public class MediationBuilderTests
{
    [Theory(DisplayName = "Build a mediation from a mediation script")]
    [InlineData(
        "SETTING " +
            "PROPAGATE UPDATE SETS " +
            "PROPAGATE ATTRIBUTE DESCRIPTIONS " +
        "DATASOURCE HumanResources VERSION \"1.0\" #Mediated human resources data from multiple sources# " +
            "WITH SCHEMA PublicSchema #Default schema about human resources#" +
                "WITH TABLEAU People #Data about people#" +
                    "WITH ATTRIBUTES " +
                        "Id #Person identifier#, " +
                        "FirstName #Person's first name#, " +
                        "LastName #Person's last name#, " +
                        "DoB #Person's date of birth#," +
                        "BirthPlace #Person's birth place#" +
                    "BEING " +
                        "SELECT enterprise1.public.people.id, enterprise1.public.people.first_name, enterprise1.public.people.last_name, enterprise2.public.people_births.dob, enterprise2.public.people_births.place_of_birth " +
                        "FROM enterprise1.public.people " +
                            "JOIN enterprise2.public.people_births " +
                                "ON enterprise1.public.people.id == enterprise2.public.people_births.person_id " +
                "WITH TABLEAU Departments #Data about departments# " +
                    "WITH ATTRIBUTES " +
                        "Id #Department identifier#, " +
                        "Name " +
                    "BEING " +
                        "SELECT enterprise3.public.departments.id_department, enterprise3.public.departments.department_name " +
                        "FROM enterprise3.public.departments " +
            "WITH SCHEMA EmptySchema #This is an empty schema#"
        )]
    public void BuildMediation(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        MediationLanguageLexer lexer = new MediationLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        MediationLanguageParser parser = new MediationLanguageParser(commonTokenStream);

        MediationLanguageListener parseListener = new MediationLanguageListener(_availableDataSources);
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var parsedMediation = parser.datasource_mediation();

        var mediation = parseListener.GenerateMediation();

        Assert.Empty(errorListener.Errors);
        Assert.True(mediation);
    }

    [Theory(DisplayName = "Build a mediation from a script and reversely-generate the script from the mediation")]
    [InlineData(
    "SETTING " +
        "PROPAGATE UPDATE SETS " +
        "PROPAGATE ATTRIBUTE DESCRIPTIONS " +
    "DATASOURCE HumanResources VERSION \"1.0\" #Mediated human resources data from multiple sources# " +
        "WITH SCHEMA PublicSchema #Default schema about human resources# " +
            "WITH TABLEAU People #Data about people# " +
                "WITH ATTRIBUTES " +
                    "Id #Person identifier#, " +
                    "FirstName #Person's first name#, " +
                    "LastName #Person's last name#, " +
                    "DoB #Person's date of birth#, " +
                    "BirthPlace #Person's birth place# " +
                "BEING " +
                    "SELECT enterprise1.public.people.id, enterprise1.public.people.first_name, enterprise1.public.people.last_name, enterprise2.public.people_births.dob, enterprise2.public.people_births.place_of_birth " +
                    "FROM enterprise1.public.people " +
                        "JOIN enterprise2.public.people_births " +
                            "ON enterprise1.public.people.id == enterprise2.public.people_births.person_id " +
            "WITH TABLEAU Departments #Data about departments# " +
                "WITH ATTRIBUTES " +
                    "Id #Department identifier#, " +
                    "Name " +
                "BEING " +
                    "SELECT enterprise3.public.departments.id_department, enterprise3.public.departments.department_name " +
                    "FROM enterprise3.public.departments " +
        "WITH SCHEMA EmptySchema #This is an empty schema# "
    )]
    public void RoundTripMediationScript(string testText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(testText);
        MediationLanguageLexer lexer = new MediationLanguageLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        MediationLanguageParser parser = new MediationLanguageParser(commonTokenStream);

        MediationLanguageListener parseListener = new MediationLanguageListener(_availableDataSources);
        VerboseErrorListener errorListener = new VerboseErrorListener();

        parser.AddParseListener(parseListener);
        parser.AddErrorListener(errorListener);

        var parsedMediation = parser.datasource_mediation();

        var mediation = parseListener.GenerateMediation();

        var mediationScript = mediation.Data?.ToMediationScript();

        var mediationScriptTrimmed = mediationScript?.Replace("\t", "").Replace("\n", " ");

        Assert.Empty(errorListener.Errors);
        Assert.True(mediation);
        Assert.NotNull(mediationScript);
        Assert.Equal(testText, mediationScriptTrimmed);
    }

    private readonly IEnumerable<DataSource> _availableDataSources =
        new List<DataSource> { 
            SchemaModelBuilder.InitDataSource("enterprise1")
                .AddSchema("public", schemaBuilder => 
                    schemaBuilder.AddTableau("people", tableauBuilder => 
                        tableauBuilder.AddAttribute("id", attrBuilder => attrBuilder.WithIsIdentity(true)
                                                                                    .WithIsNullable(false)
                                                                                    .WithDataType(DataTypes.INT))
                                      .AddAttribute("first_name", attrBuilder => attrBuilder.WithDataType(DataTypes.STRING))
                                      .AddAttribute("last_name", attrBuilder => attrBuilder.WithDataType(DataTypes.STRING))
                                      .WithDefaultUpdateSet()
                    )
                )
                .Build(),
            SchemaModelBuilder.InitDataSource("enterprise2")
                .AddSchema("public", schemaBuilder =>
                    schemaBuilder.AddTableau("people_births", tableauBuilder =>
                        tableauBuilder.AddAttribute("person_id", attrBuilder => attrBuilder.WithIsIdentity(true)
                                                                                           .WithIsNullable(false)
                                                                                           .WithDataType(DataTypes.INT))
                                      .AddAttribute("dob", attrBuilder => attrBuilder.WithDataType(DataTypes.DATETIME))
                                      .AddAttribute("place_of_birth", attrBuilder => attrBuilder.WithDataType(DataTypes.STRING))
                                      .AddUpdateSet(conf => conf.WithAttributesNamed("person_id", "place_of_birth"))
                    )
                )
                .Build(),
            SchemaModelBuilder.InitDataSource("enterprise3")
                .AddSchema("public", schemaBuilder =>
                    schemaBuilder.AddTableau("departments", tableauBuilder =>
                        tableauBuilder.AddAttribute("id_department", attrBuilder => attrBuilder.WithIsIdentity(true)
                                                                                               .WithIsNullable(false)
                                                                                               .WithDescription("PK AI INT Id for departments")
                                                                                               .WithDataType(DataTypes.INT))
                                      .AddAttribute("department_name", attrBuilder => attrBuilder.WithDescription("Department name")
                                                                                                 .WithDataType(DataTypes.STRING))
                                      .WithDefaultUpdateSet()
                    )
                )
                .Build()
        };
}
