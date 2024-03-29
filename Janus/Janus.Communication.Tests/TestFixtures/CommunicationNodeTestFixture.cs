﻿using Janus.Commons.CommandModels;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Communication.Tests.Mocks;
using Janus.Serialization;
using Microsoft.Extensions.Configuration;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.Communication.Tests.TestFixtures;

public abstract class CommunicationNodeTestFixture
{
    private readonly IConfiguration _configuration;

    public IReadOnlyDictionary<string, CommunicationNodeOptions> MaskCommunicationNodeOptions => _maskCommunicationNodeOptions;
    public IReadOnlyDictionary<string, CommunicationNodeOptions> MediatorCommunicationNodeOptions => _mediatorCommunicationNodeOptions;
    public IReadOnlyDictionary<string, CommunicationNodeOptions> WrapperCommunicationNodeOptions => _wrapperCommunicationNodeOptions;

    private readonly Dictionary<string, CommunicationNodeOptions> _maskCommunicationNodeOptions;
    private readonly Dictionary<string, CommunicationNodeOptions> _mediatorCommunicationNodeOptions;
    private readonly Dictionary<string, CommunicationNodeOptions> _wrapperCommunicationNodeOptions;

    public abstract IBytesSerializationProvider SerializationProvider { get; }

    public CommunicationNodeTestFixture()
    {
        _maskCommunicationNodeOptions = new Dictionary<string, CommunicationNodeOptions>();
        _mediatorCommunicationNodeOptions = new Dictionary<string, CommunicationNodeOptions>();
        _wrapperCommunicationNodeOptions = new Dictionary<string, CommunicationNodeOptions>();

        _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();


        var options = LoadCommunicationNodeOptions(_configuration);

        _maskCommunicationNodeOptions = options.maskNodeOptions.ToDictionary(o => o.key, o => o.Item2);
        _mediatorCommunicationNodeOptions = options.mediatorNodeOptions.ToDictionary(o => o.key, o => o.Item2);
        _wrapperCommunicationNodeOptions = options.wrapperNodeOptions.ToDictionary(o => o.key, o => o.Item2);
    }

    public MaskCommunicationNode GetMaskCommunicationNode(string nodeId)
        => CommunicationNodes.CreateTcpMaskCommunicationNode(_maskCommunicationNodeOptions[nodeId], SerializationProvider);

    public MediatorCommunicationNode GetMediatorCommunicationNode(string nodeId)
        => CommunicationNodes.CreateTcpMediatorCommunicationNode(_mediatorCommunicationNodeOptions[nodeId], SerializationProvider);

    public WrapperCommunicationNode GetWrapperCommunicationNode(string nodeId)
        => CommunicationNodes.CreateTcpWrapperCommunicationNode(_wrapperCommunicationNodeOptions[nodeId], SerializationProvider);

    private (
        IEnumerable<(string key, CommunicationNodeOptions)> maskNodeOptions,
        IEnumerable<(string key, CommunicationNodeOptions)> mediatorNodeOptions,
        IEnumerable<(string key, CommunicationNodeOptions)> wrapperNodeOptions)
        LoadCommunicationNodeOptions(IConfiguration configuration)
    {
        return (
            configuration.GetChildren()
                         .ToList()
                         .SingleOrDefault(section => section.Key.Equals("Nodes"))?
                         .GetChildren()
                         .SingleOrDefault(section => section.Key.Equals("Masks"))?
                         .GetChildren()
                         .Select(maskSection => (maskSection.Key, maskSection.GetCommunicationNodeOptions()))
                         .ToList() ?? new List<(string key, CommunicationNodeOptions)>(),
            configuration.GetChildren()
                         .ToList()
                         .SingleOrDefault(section => section.Key.Equals("Nodes"))?
                         .GetChildren()
                         .SingleOrDefault(section => section.Key.Equals("Mediators"))?
                         .GetChildren()
                         .Select(mediatorSection => (mediatorSection.Key, mediatorSection.GetCommunicationNodeOptions()))
                         .ToList() ?? new List<(string key, CommunicationNodeOptions)>(),
            configuration.GetChildren()
                         .ToList()
                         .SingleOrDefault(section => section.Key.Equals("Nodes"))?
                         .GetChildren()
                         .SingleOrDefault(section => section.Key.Equals("Wrappers"))?
                         .GetChildren()
                         .Select(wrapperSection => (wrapperSection.Key, wrapperSection.GetCommunicationNodeOptions()))
                         .ToList() ?? new List<(string key, CommunicationNodeOptions)>()
                     );

    }

    public MediatorCommunicationNode GetUnresponsiveMediator()
    => CommunicationNodes.CreateMediatorCommunicationNode(
        MediatorCommunicationNodeOptions["MediatorUnresponsive"],
        new AlwaysTimeoutTcpNetworkAdapter(MediatorCommunicationNodeOptions["MediatorUnresponsive"].ListenPort, SerializationProvider)
        );

    public DataSource GetSchema()
       => SchemaModelBuilder.InitDataSource("dataSource")
                .AddSchema("schema1",
                    schemaConf => schemaConf.AddTableau("tableau1",
                        tableauConf => tableauConf.AddAttribute("attr1", attrConf => attrConf.WithDataType(DataTypes.INT)
                                                                                             .WithIsNullable(false))
                                                  .AddAttribute("attr2", attrConf => attrConf.WithDataType(DataTypes.STRING)
                                                                                             .WithIsNullable(true))
                                                  .AddAttribute("attr3", attrConf => attrConf.WithDataType(DataTypes.DECIMAL)
                                                                                             .WithIsNullable(false))
                                                  .AddAttribute("attr4", attrConf => attrConf.WithDataType(DataTypes.INT)
                                                                                             .WithIsNullable(false)
                                                                                             .WithIsIdentity(true))
                                                  .WithDefaultUpdateSet())
                    .AddTableau("tableau2",
                        tableauConf => tableauConf.AddAttribute("attr1", attrConf => attrConf.WithDataType(DataTypes.INT)
                                                                                             .WithIsNullable(false))
                                                  .AddAttribute("attr2", attrConf => attrConf.WithDataType(DataTypes.STRING)
                                                                                             .WithIsNullable(false))
                                                  .AddAttribute("attr3", attrConf => attrConf.WithDataType(DataTypes.DECIMAL)
                                                                                             .WithIsNullable(false))
                                                  .WithDefaultUpdateSet()))
                .Build();

    public Query GetQuery()
        => QueryModelBuilder.InitQueryOnDataSource("dataSource.schema1.tableau1", GetSchema())
            .WithName("testQuery")
            .WithJoining(conf => conf.AddJoin("dataSource.schema1.tableau1.attr1", "dataSource.schema1.tableau2.attr1"))
            .WithSelection(conf => conf.WithExpression(LT("dataSource.schema1.tableau1.attr3", 7.0)))
            .WithProjection(conf => conf.AddAttribute("dataSource.schema1.tableau1.attr1")
                                        .AddAttribute("dataSource.schema1.tableau1.attr2")
                                        .AddAttribute("dataSource.schema1.tableau1.attr3")
                                        .AddAttribute("dataSource.schema1.tableau2.attr1")
                                        .AddAttribute("dataSource.schema1.tableau2.attr3"))
            .Build();

    public TabularData GetQueryResultData()
    {
        return TabularDataBuilder
                .InitTabularData(new()
                {
                { "dataSource.schema1.tableau1.attr1", DataTypes.INT },
                { "dataSource.schema1.tableau1.attr2", DataTypes.STRING},
                { "dataSource.schema1.tableau1.attr3", DataTypes.DECIMAL},
                { "dataSource.schema1.tableau2.attr1", DataTypes.INT },
                { "dataSource.schema1.tableau2.attr3", DataTypes.DECIMAL }
                })
                .WithName("testData")
                .AddRow(conf => conf.WithRowData(new()
                {
                { "dataSource.schema1.tableau1.attr1", 1 },
                { "dataSource.schema1.tableau1.attr2", "HELLO1"},
                { "dataSource.schema1.tableau1.attr3", 2.0},
                { "dataSource.schema1.tableau2.attr1", 4 },
                { "dataSource.schema1.tableau2.attr3", 2.7 }
                }))
                .AddRow(conf => conf.WithRowData(new()
                {
                { "dataSource.schema1.tableau1.attr1", 2 },
                { "dataSource.schema1.tableau1.attr2", "HELLO2"},
                { "dataSource.schema1.tableau1.attr3", 4.5},
                { "dataSource.schema1.tableau2.attr1", 5 },
                { "dataSource.schema1.tableau2.attr3", 2.3 }
                }))
                .AddRow(conf => conf.WithRowData(new()
                {
                { "dataSource.schema1.tableau1.attr1", 3 },
                { "dataSource.schema1.tableau1.attr2", "HELL3"},
                { "dataSource.schema1.tableau1.attr3", 3.3},
                { "dataSource.schema1.tableau2.attr1", 5 },
                { "dataSource.schema1.tableau2.attr3", 1.0 }
                }))
                .Build();
    }

    public InsertCommand GetInsertCommand()
    {
        var dataToInsert =
            TabularDataBuilder.InitTabularData(new() { { "attr1", DataTypes.INT }, { "attr2", DataTypes.STRING }, { "attr3", DataTypes.DECIMAL }, { "attr4", DataTypes.INT } })
                              .AddRow(conf => conf.WithRowData(new() { { "attr1", 1 }, { "attr2", "TEST_STRING" }, { "attr3", 1.2 }, { "attr4", 0 } }))
                              .AddRow(conf => conf.WithRowData(new() { { "attr1", 2 }, { "attr2", null }, { "attr3", 2.3 }, { "attr4", 1 } }))
                              .Build();

        var insertCommand =
            InsertCommandBuilder.InitOnDataSource("dataSource.schema1.tableau1", GetSchema())
                                .WithInstantiation(conf => conf.WithValues(dataToInsert))
                                .Build();
        return insertCommand;
    }
}
