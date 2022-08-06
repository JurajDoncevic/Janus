using CommandLine;
using Janus.Commons;
using Janus.Communication;
using Janus.Communication.Nodes.Implementations;
using Janus.Wrapper;
using Janus.Wrapper.Sqlite.ConsoleApp;
using Janus.Serialization;
using Janus.Serialization.Avro;
using Janus.Serialization.Bson;
using Janus.Serialization.MongoBson;
using Janus.Serialization.Protobufs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Extensions.Hosting;
using NLog.Extensions.Logging;
using Janus.Wrapper.Sqlite;
using Janus.Wrapper.Sqlite.Translation;
using Janus.Wrapper.Sqlite.LocalQuerying;
using Janus.Wrapper.Sqlite.SchemaInferrence;
using Janus.Wrapper.SchemaInferrence;

// get the application options
var applicationOptions =
    Parser.Default.ParseArguments<ApplicationOptions>(args) // get options from the command line
        .WithParsed(options =>
        {

        })
        .WithNotParsed(errors =>
        {
        }).Value;

// configure host
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostContext, configurationBuilder) =>
    {
        // get the settings file path
        if (applicationOptions.SettingsFilePath != null)
        {
            configurationBuilder.AddJsonFile(applicationOptions.SettingsFilePath); // create IConfig from the settings file
        }
    })
    .ConfigureLogging((hostContext, loggingBuilder) => // setup logging
    {
        var loggingSection = hostContext.Configuration.GetSection("NLog");
        if (loggingSection != null)
        {
            LogManager.Configuration = new NLogLoggingConfiguration(loggingSection);
        }

    }).UseNLog()
    .ConfigureServices((hostContext, services) => // configure services and injections
    {
        // get the mediator options from the ComponentOptions section
        var wrapperOptions = hostContext.Configuration
                                .GetSection("ComponentOptions")
                                .Get<WrapperConfigurationOptions>()
                                .ToWrapperOptions();


        services.AddSingleton(hostContext.Configuration); // register the IConfig
        services.AddSingleton<Janus.Logging.ILogger, Janus.Logging.Logger>(); // register the concrete logger

        // check if the given data format and network adapter type are compatible
        if (!Utils.IsDataFormatCompatibleWithAdapter(
            wrapperOptions.CommunicationFormat,
            wrapperOptions.NetworkAdapterType))
            throw new Exception($"Incompatible communication data format {wrapperOptions.CommunicationFormat} for network adapter type {wrapperOptions.NetworkAdapterType}");

        // set the serialization provider, network adapter and communication node
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
        services.AddSingleton<SqliteDataTranslator>();
        services.AddSingleton<SqliteQueryExecutor>();
        services.AddSingleton<SqliteWrapperQueryManager>();
        
        // setup commanding
        services.AddSingleton<SqliteWrapperCommandManager>();

        // setup schema managment
        services.AddSingleton<SqliteSchemaModelProvider>();
        services.AddSingleton<SchemaInferrer>(
            provider => new SchemaInferrer(provider.GetService<SqliteSchemaModelProvider>()!, wrapperOptions.NodeId));
        services.AddSingleton<SqliteWrapperSchemaManager>();
        
        // setup controller
        services.AddSingleton<SqliteWrapperController>();

        services.AddSingleton<WrapperOptions>(wrapperOptions);
        services.AddSingleton<ApplicationOptions>(applicationOptions);
        services.AddSingleton<Application>();

    })
    .Build();

await host.StartAsync();
host.Services.GetService<Application>();
host.WaitForShutdown();
