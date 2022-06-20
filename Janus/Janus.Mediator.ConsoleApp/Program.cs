using CommandLine;
using Janus.Communication;
using Janus.Communication.NetworkAdapters;
using Janus.Communication.NetworkAdapters.Tcp;
using Janus.Communication.Nodes;
using Janus.Communication.Nodes.Implementations;
using Janus.Communication.Remotes;
using Janus.Mediator.ConsoleApp;
using Janus.Mediator.ConsoleApp.Options;
using Janus.Mediator.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Extensions.Hosting;
using NLog.Extensions.Logging;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostContext, configurationBuilder) =>
    {
        Parser.Default.ParseArguments<CommandLineOptions>(args)
            .WithParsed(options =>
            {
                if (options.ConfigurationFilePath != null) // if the configuration file is given
                {
                    configurationBuilder.AddJsonFile(options.ConfigurationFilePath);
                }
                else // the configuration is given via CL arguments
                {
                    configurationBuilder.AddInMemoryCollection(
                        new Dictionary<string, string>
                        {
                            { "ComponentOptions:NodeId", options.NodeId },
                            { "ComponentOptions:ListenPort", options.ListenPort.ToString() },
                            { "ComponentOptions:TimeoutMs", options.TimeoutMs.ToString() },
                            { "ComponentOptions:StartupRemotePoints", "[]" },
                        });
                }

                configurationBuilder.AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        { "ApplicationOptions:StartWithCLI", options.StartWithCLI.ToString() }
                    });
            })
            .WithNotParsed(errors =>
            {
            });
    })
    .ConfigureLogging((hostContext, loggingBuilder) =>
    {
        var loggingSection = hostContext.Configuration.GetSection("NLog");
        if (loggingSection != null)
        {
            LogManager.Configuration = new NLogLoggingConfiguration(loggingSection);
        }

    }).UseNLog()
    .ConfigureServices((hostContext, services) =>
    {
        var mediatorOptions = hostContext.Configuration
                                .GetSection("ComponentOptions")
                                .Get<MediatorConfigurationOptions>()
                                .ToMediatorOptions();

        var applicationOptions = hostContext.Configuration
                                    .GetSection("ApplicationOptions")
                                    .Get<ApplicationConfigurationOptions>()
                                    .ToApplicationOptions();

        services.AddSingleton<IConfiguration>(hostContext.Configuration);
        services.AddSingleton<Janus.Utils.Logging.ILogger, Janus.Utils.Logging.Logger>();
        services.AddSingleton<MediatorCommunicationNode>(serviceProvider =>
            CommunicationNodes.CreateTcpMediatorCommunicationNode(
                new Janus.Communication.Nodes.CommunicationNodeOptions(
                    mediatorOptions.NodeId,
                    mediatorOptions.ListenPort,
                    mediatorOptions.TimeoutMs
                    ),
                serviceProvider.GetService<Janus.Utils.Logging.ILogger>()
                )
            );
        services.AddSingleton<MediatorQueryManager>();
        services.AddSingleton<MediatorCommandManager>();
        services.AddSingleton<MediatorSchemaManager>();
        services.AddSingleton<MediatorController>();
        services.AddSingleton<MediatorOptions>(mediatorOptions);
        services.AddSingleton<ApplicationOptions>(applicationOptions);
        services.AddSingleton<Application>();
    })
    .Build();
host.Start();
host.Services.GetService<Application>();