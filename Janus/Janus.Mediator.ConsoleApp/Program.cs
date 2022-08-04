using CommandLine;
using Janus.Commons;
using Janus.Communication;
using Janus.Communication.Nodes.Implementations;
using Janus.Mediator.ConsoleApp;
using Janus.Mediator;
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
        var mediatorOptions = hostContext.Configuration
                                .GetSection("ComponentOptions")
                                .Get<MediatorConfigurationOptions>()
                                .ToMediatorOptions();


        services.AddSingleton(hostContext.Configuration); // register the IConfig
        services.AddSingleton<Janus.Logging.ILogger, Janus.Logging.Logger>(); // register the concrete logger

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
        services.AddSingleton<MediatorController>();
        services.AddSingleton<MediatorOptions>(mediatorOptions);
        services.AddSingleton<ApplicationOptions>(applicationOptions);
        services.AddSingleton<Application>();

    })
    .Build();
await host.StartAsync();
host.Services.GetService<Application>();
host.WaitForShutdown();
