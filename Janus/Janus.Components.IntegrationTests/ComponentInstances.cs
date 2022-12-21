using Janus.Commons;
using Janus.Communication;
using Janus.Communication.Nodes.Implementations;
using Janus.Components.Persistence;
using Janus.Mediator;
using Janus.Mediator.Persistence;
using Janus.Mediator.Persistence.LiteDB;
using Janus.Serialization;
using Janus.Serialization.Avro;
using Janus.Serialization.Bson;
using Janus.Serialization.Json;
using Janus.Serialization.MongoBson;
using Janus.Serialization.Protobufs;
using Janus.Wrapper;
using Janus.Wrapper.Persistence;
using Janus.Wrapper.SchemaInferrence;
using Janus.Wrapper.Sqlite.LocalCommanding;
using Janus.Wrapper.Sqlite.LocalQuerying;
using Janus.Wrapper.Sqlite.SchemaInferrence;
using Janus.Wrapper.Sqlite.Translation;
using Janus.Wrapper.Sqlite;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Components.IntegrationTests;
internal class ComponentInstances
{
    private static Dictionary<Guid, IHost> _hostedComponents = new Dictionary<Guid, IHost>();

    internal static void DisposeOfComponent(Guid guid)
    {
        var component = _hostedComponents[guid];
        component?.StopAsync().Wait();
        component?.Dispose();

        if (component != null)
        {
            _hostedComponents.Remove(guid);
        }
    }

    internal static (Guid hostId, MediatorManager mediatorManager) CreateMediatorHostedInstance(MediatorOptions mediatorOptions)
    {
        var builder = Host.CreateDefaultBuilder();
        builder.ConfigureServices(services =>
        {
            // check if the given data format and network adapter type are compatible
            if (!Utils.IsDataFormatCompatibleWithAdapter(
                mediatorOptions.CommunicationFormat,
                mediatorOptions.NetworkAdapterType))
                throw new Exception($"Incompatible communication data format {mediatorOptions.CommunicationFormat} for network adapter type {mediatorOptions.NetworkAdapterType}");

            // set the serialization provider and network adapter
            var _ =
            mediatorOptions.NetworkAdapterType switch
            {
                NetworkAdapterTypes.TCP =>
                    services
                    .AddSingleton<IBytesSerializationProvider>(
                        mediatorOptions.CommunicationFormat switch
                        {
                            CommunicationFormats.AVRO => new AvroSerializationProvider(),
                            CommunicationFormats.BSON => new BsonSerializationProvider(),
                            CommunicationFormats.MONGO_BSON => new MongoBsonSerializationProvider(),
                            CommunicationFormats.PROTOBUFS => new ProtobufsSerializationProvider(),
                            _ => new AvroSerializationProvider()
                        })
                    .AddSingleton<MediatorCommunicationNode>(serviceProvider =>
                        CommunicationNodes.CreateTcpMediatorCommunicationNode(
                            new Janus.Communication.Nodes.CommunicationNodeOptions(
                                mediatorOptions.NodeId,
                                mediatorOptions.ListenPort,
                                mediatorOptions.TimeoutMs
                                ),
                            serviceProvider.GetService<IBytesSerializationProvider>()!,
                            serviceProvider.GetService<Janus.Logging.ILogger>())),
                _ => throw new Exception("Unknown network adapter type")
            };

            services.AddSingleton<MediatorQueryManager>();
            services.AddSingleton<MediatorCommandManager>();
            services.AddSingleton<MediatorSchemaManager>();

            services.AddSingleton<ILiteDatabase, LiteDatabase>(services => new LiteDatabase(mediatorOptions.PersistenceConnectionString));
            services.AddSingleton<JsonSerializationProvider>();
            services.AddSingleton<Janus.Mediator.Persistence.IDataSourceInfoPersistence, Janus.Mediator.Persistence.LiteDB.DataSourceInfoPersistence>();
            services.AddSingleton<IRemotePointPersistence, Janus.Mediator.Persistence.LiteDB.RemotePointPersistence>();
            services.AddSingleton<MediatorPersistenceProvider>();

            services.AddSingleton<MediatorOptions>(mediatorOptions);

            services.AddSingleton<MediatorManager>();
        });



        var app = builder.Build();

        var hostGuid = Guid.NewGuid();

        _hostedComponents.Add(hostGuid, app);

        var startTask = app.StartAsync();

        var mediatorManager = app.Services.GetService<MediatorManager>();

        return (hostGuid, mediatorManager ?? throw new Exception("Couldn't get the required mediator manager"));
    }

    internal static (Guid hostId, SqliteWrapperManager wrapperManager) CreateSqliteWrapperHostedInstance(WrapperOptions wrapperOptions)
    {
        var builder = Host.CreateDefaultBuilder();
        builder.ConfigureServices(services =>
        {
            // check if the given data format and network adapter type are compatible
            if (!Utils.IsDataFormatCompatibleWithAdapter(
                wrapperOptions.CommunicationFormat,
                wrapperOptions.NetworkAdapterType))
                throw new Exception($"Incompatible communication data format {wrapperOptions.CommunicationFormat} for network adapter type {wrapperOptions.NetworkAdapterType}");

            // set the serialization provider and network adapter
            var _ =
            wrapperOptions.NetworkAdapterType switch
            {
                NetworkAdapterTypes.TCP =>
                    services
                    .AddSingleton<IBytesSerializationProvider>(
                        wrapperOptions.CommunicationFormat switch
                        {
                            CommunicationFormats.AVRO => new AvroSerializationProvider(),
                            CommunicationFormats.BSON => new BsonSerializationProvider(),
                            CommunicationFormats.MONGO_BSON => new MongoBsonSerializationProvider(),
                            CommunicationFormats.PROTOBUFS => new ProtobufsSerializationProvider(),
                            _ => new AvroSerializationProvider()
                        })
                    .AddSingleton<WrapperCommunicationNode>(serviceProvider =>
                        CommunicationNodes.CreateTcpWrapperCommunicationNode(
                            new Janus.Communication.Nodes.CommunicationNodeOptions(
                                wrapperOptions.NodeId,
                                wrapperOptions.ListenPort,
                                wrapperOptions.TimeoutMs
                                ),
                            serviceProvider.GetService<IBytesSerializationProvider>()!,
                            serviceProvider.GetService<Janus.Logging.ILogger>())),
                _ => throw new Exception("Unknown network adapter type")
            };

            // setup querying
            services.AddSingleton<SqliteQueryTranslator>();
            services.AddSingleton<SqliteDataTranslator>(provider => new SqliteDataTranslator(wrapperOptions.DataSourceName ?? wrapperOptions.NodeId));
            services.AddSingleton<SqliteQueryExecutor>(provider => new SqliteQueryExecutor(wrapperOptions.SourceConnectionString));
            services.AddSingleton<SqliteWrapperQueryManager>();

            // setup commanding
            services.AddSingleton<SqliteCommandTranslator>();
            services.AddSingleton<SqliteCommandExecutor>(provider => new SqliteCommandExecutor(wrapperOptions.SourceConnectionString));
            services.AddSingleton<SqliteWrapperCommandManager>();

            // setup schema managment
            services.AddSingleton<SqliteSchemaModelProvider>(provider => new SqliteSchemaModelProvider(wrapperOptions.SourceConnectionString));
            services.AddSingleton<SchemaInferrer>(
                provider => new SchemaInferrer(provider.GetService<SqliteSchemaModelProvider>()!, wrapperOptions.DataSourceName ?? wrapperOptions.NodeId));
            services.AddSingleton<SqliteWrapperSchemaManager>();

            // setup controller
            services.AddSingleton<SqliteWrapperManager>();

            services.AddSingleton<ILiteDatabase, LiteDatabase>(services => new LiteDatabase(wrapperOptions.PersistenceConnectionString));
            services.AddSingleton<JsonSerializationProvider>();
            services.AddSingleton<Janus.Wrapper.Persistence.IDataSourceInfoPersistence, Janus.Wrapper.Persistence.LiteDB.DataSourceInfoPersistence>();
            services.AddSingleton<IRemotePointPersistence, Janus.Wrapper.Persistence.LiteDB.RemotePointPersistence>();
            services.AddSingleton<WrapperPersistenceProvider>();

            services.AddSingleton<WrapperOptions>(wrapperOptions);

            services.AddSingleton<SqliteWrapperManager>();
        });

        var app = builder.Build();

        var hostGuid = Guid.NewGuid();

        _hostedComponents.Add(hostGuid, app);

        var startTask = app.StartAsync();

        var wrapperManager = app.Services.GetService<SqliteWrapperManager>();

        return (hostGuid, wrapperManager ?? throw new Exception("Couldn't get the required sqlite wrapper manager"));
    }
}
