using Antlr4.Runtime.Misc;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Mediation.MediationModels;
using Janus.Mediation.MediationModels.Building;
using Janus.Mediation.MediationModels.MediationQueryModels;
using Janus.Mediation.MediationModels.SourceQueryModels.Building;

namespace Janus.MediationLanguage;
public class MediationLanguageListener : MediationLanguageBaseListener
{
    private IDataSourceMediationBuilder? _dataSourceMediationBuilder;
    private readonly IEnumerable<DataSource> _availableDataSources;

    private string? _dataSourceName;
    private string? _dataSourceVersion;
    private string? _dataSourceDescription;

    private List<SchemaConfiguration> _schemaConfigurations = new();
    private List<TableauConfiguration> _currentTableauConfigurations = new();
    private SourceQuery? _currentSourceQuery;

    public MediationLanguageListener(IEnumerable<DataSource> availableDataSources)
    {
        _availableDataSources = availableDataSources;
    }

    public Result<DataSourceMediation> GenerateMediation()
    {
        return _dataSourceMediationBuilder?.Build() ?? Result<DataSourceMediation>.OnFailure();
    }

    public override void ExitDatasource_mediation([NotNull] MediationLanguageParser.Datasource_mediationContext context)
    {
        bool propagateAttrDescriptions = context.setting_clause()?.PROP_ATTR_DESCR_KW() is not null;
        bool propagateUpdateSets = context.setting_clause()?.PROP_UPDATE_SETS_KW() is not null;

        _dataSourceName = context.STRUCTURE_NAME().GetText().Trim();
        _dataSourceVersion = context.version_expr()?.STRING().GetText().Trim('"');
        _dataSourceDescription = context.description_expr()?.DESCRIPTION_TEXT().GetText().Trim('#');

        _dataSourceMediationBuilder = 
        _schemaConfigurations.Fold(MediationBuilder.ForDataSource(_dataSourceName, _availableDataSources),
            (schemaConf, dsBuilder) =>
                dsBuilder.WithSchema(schemaConf.SchemaName, schemaBuilder =>
                    schemaConf.TableauConfigurations.Fold(schemaBuilder,
                        (tableauConf, scBuilder) => scBuilder.WithTableau(tableauConf.TableauName,
                            tableauBuilder => tableauConf.AttributeConfigurations.Fold(tableauBuilder,
                                (attributeConf, tblBuilder) => tblBuilder.WithDeclaredAttribute(attributeConf.AttributeName, attributeConf.AttributeDescription))
                                                                         .WithQuery(tableauConf.SourceQuery)
                                                                         .WithDescription(tableauConf.TableauDescription)
                                                                         )
                        ).WithDescription(schemaConf.SchemaDescription)
                    )
                )
                .WithVersion(_dataSourceVersion!)
                .WithDescription(_dataSourceDescription!)
                .WithAttributeDescriptionPropagation(propagateAttrDescriptions)
                .WithUpdateSetPropagation(propagateUpdateSets);

        base.ExitDatasource_mediation(context);
    }

    public override void EnterSchema_mediation([NotNull] MediationLanguageParser.Schema_mediationContext context)
    {
        _currentTableauConfigurations.Clear();
        base.EnterSchema_mediation(context);
    }

    public override void ExitSchema_mediation([NotNull] MediationLanguageParser.Schema_mediationContext context)
    {
        var schemaName = context.STRUCTURE_NAME().GetText().Trim();
        var schemaDescription = context.description_expr()?.DESCRIPTION_TEXT().GetText().Trim('#');

        var currentSchemaConfiguration = new SchemaConfiguration
        {
            SchemaName = schemaName,
            SchemaDescription = schemaDescription,
            TableauConfigurations =  new (_currentTableauConfigurations)
        };

        _schemaConfigurations.Add(currentSchemaConfiguration);

        base.ExitSchema_mediation(context);
    }


    public override void ExitSource_query_clause([NotNull] MediationLanguageParser.Source_query_clauseContext context)
    {
        var projectionAttrIds = context.select_clause().projection_expr().ATTRIBUTE_ID().Select(attrId => attrId.GetText().Trim()).ToList();

        var initialTableauId = context.from_clause().TABLEAU_ID().GetText().Trim();

        var joins = new List<(string fkAttrId, string pkAttrId)>();

        foreach (var joinContext in context.from_clause().join_clause())
        {
            var pkTableauId = joinContext.TABLEAU_ID().GetText().Trim();
            // attr ids in equi join
            var firstAttrId = joinContext.ATTRIBUTE_ID()[0].GetText().Trim();
            var secondAttrId = joinContext.ATTRIBUTE_ID()[1].GetText().Trim();

            (var pkAttrId, var fkAttrId) =
                firstAttrId.StartsWith(pkTableauId)
                    ? (firstAttrId, secondAttrId)
                    : (secondAttrId, firstAttrId);

            joins.Add((fkAttrId, pkAttrId));
        }

        _currentSourceQuery =
            SourceQueryBuilder
                .InitQueryOn(initialTableauId, _availableDataSources)
                .WithJoining(configuration => joins.Fold(configuration, (join, conf) => conf.AddJoin(join.fkAttrId, join.pkAttrId)))
                .WithProjection(configuration => projectionAttrIds.Fold(configuration, (attrId, conf) => conf.AddAttribute(attrId)))
                .Build();

        base.ExitSource_query_clause(context);
    }

    public override void ExitTableau_mediation([NotNull] MediationLanguageParser.Tableau_mediationContext context)
    {
        var tableauConfiguration = new TableauConfiguration
        {
            TableauName = context.STRUCTURE_NAME().GetText().Trim(),
            TableauDescription = context.description_expr().GetText().Trim('#'),
            AttributeConfigurations = context.attribute_mediation()
                                    .attribute_declaration()
                                    .Select(ctx => new AttributeConfiguration
                                    {
                                        AttributeName = ctx.STRUCTURE_NAME().GetText().Trim(),
                                        AttributeDescription = ctx.description_expr()?.GetText().Trim('#')
                                    })
                                    .ToList(),
            SourceQuery = _currentSourceQuery!
        };

        _currentTableauConfigurations.Add(tableauConfiguration);
        base.ExitTableau_mediation(context);
    }

    private class SchemaConfiguration
    {
        public string SchemaName { get; set; } = string.Empty;
        public string? SchemaDescription { get; set; } = null;
        public List<TableauConfiguration> TableauConfigurations { get; set; } = new();
    }

    private class TableauConfiguration
    {
        public string TableauName { get; set; } = string.Empty;
        public string? TableauDescription { get; set; } = null;
        public List<AttributeConfiguration> AttributeConfigurations { get; set; } = new();
        public SourceQuery SourceQuery { get; set; }
    }

    private class AttributeConfiguration
    {
        public string AttributeName { get; set; } = string.Empty;
        public string? AttributeDescription { get; set; } = null;
    }
}
